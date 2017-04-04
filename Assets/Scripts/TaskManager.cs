using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void TaskChange();

public class TaskManager : MonoBehaviour {

    private static TaskManager instance;
    public bool IsFullPreflight;
    public static event TaskChange OnStartTasks;
    public static event TaskChange OnTaskChanged;
    public static event TaskChange OnEndTasks;
    public static event TaskChange OnCardChanged;

    private static Card[] cards;
    public static Task PreviousTask;
    
    public static int CardsNumber { get; private set; }
    public static int CardIndex { get; private set; }
    public static int TaskIndex { get; private set; }
    public static int TasksToSign { get; private set; }

    public static Task CurrentTask
    {
        get {
			return (CardIndex < cards.Length && TaskIndex < CurrentCard.tasks.Length) ? cards[CardIndex].tasks[TaskIndex] : null;
		}
    }

    public static Card CurrentCard
    {
        get { return CardIndex < cards.Length ? cards[CardIndex] : null; }
    }

	public static Card GetCard(int index)
	{
		return cards[index];
	}
	
    void Awake () {
        instance = this;
        InitTaskManager();
    }

    private static void InitTaskManager()
    {
        if(!TextsBridge.IsTasksLoaded())
        {
            TextsBridge.SetTaskFileName(instance.IsFullPreflight);
        }

        cards = TextsBridge.GetCards();

        if (cards != null)
        {
            PrepareTaskParameters();
            CardsNumber = cards.Length;
            PreviousTask = CurrentTask;
            CardIndex = 0;
            TaskIndex = 0;
        }
    }

    private void Start()
    {
        if(PreviousTask == null) PreviousTask = CurrentTask;
        if (OnStartTasks != null) 
            OnStartTasks();
    }

    public static int nextTask()
    {
        return changeTask(true);
    }

    public static int prevTask()
    {
        return changeTask(false);
    }

    public static int changeTask(bool isNext)
    {
        PreviousTask = CurrentTask;
        
        if(isNext)
        {
            if (TaskIndex >= CurrentCard.tasks.Length - 1) // Goes to next card 
            {
				changeCard(isNext, false);
            }
            else
            {
                TaskIndex += 1;
            }
        } else // previous task
        {
            if(TaskIndex == 0 && CardIndex > 0) // Goes to previous card 
            {
				changeCard(isNext, false);
            } else if(TaskIndex > 0)
            {
                TaskIndex -= 1;
            }

            CurrentTask.isAlreadySigned = false;
        }

        if (OnTaskChanged != null)
            OnTaskChanged();

        return TaskIndex;
    }


    public static void check()
    {
        CurrentTask.isAlreadySigned = true;
    }

    public void NextCard()
    {
		if (CardIndex < cards.Length - 1)
		{
			if (OneTaskController.Instance.IsChangingCard())
			{
				OneTaskController.Instance.CancelChangingCard();
			}
			changeCard(true, true);
			if (OnTaskChanged != null)
				OnTaskChanged();
		}
    }

    public void PrevCard()
    {
		if (CardIndex > 0)
		{
			if (OneTaskController.Instance.IsChangingCard())
			{
				OneTaskController.Instance.CancelChangingCard();
			}
			changeCard(false, true);
			if (OnTaskChanged != null)
				OnTaskChanged();
		}
    }

	public static void changeCard(bool isNext, bool forceResetTaskIndex)
    {
        CardIndex += isNext ? 1 : -1;
		TaskIndex = (isNext || forceResetTaskIndex) ? 0 : CurrentCard.tasks.Length - 1;

        if (isFinished())
		{
			if (OnEndTasks != null)
				OnEndTasks();
		}
		else
		{
			if (OnCardChanged != null)
				OnCardChanged();
		}
    }

    public static bool isFinished()
    {
        return CardIndex >= cards.Length;
    }

    private static void PrepareTaskParameters()
    {
        // Count tasks to sign and make task to be not signed
        TasksToSign = 0;

        foreach (Card c in cards)
        {
            foreach (Task t in c.tasks)
            {
                t.isAlreadySigned = false;
                TasksToSign += t.signedTask ? 1 : 0;
            }
        }
    }

    public static int GetNumberOfSignedTasks()
    {
        int signedTasks = 0;

        if (cards == null) InitTaskManager();

        foreach (Card c in cards)
            foreach (Task t in c.tasks)
                signedTasks += t.isAlreadySigned ? 1 : 0;

        return signedTasks;
    }
}
