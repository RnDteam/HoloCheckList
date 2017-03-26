using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TaskChange();

public class TaskManager : MonoBehaviour {

    public static event TaskChange OnStartTasks;
    public static event TaskChange OnTaskChanged;
    public static event TaskChange OnEndTasks;
    public static event TaskChange OnCardChanged;

    private static Card[] cards;
    public static int nCardsNumber;
    public static int nCardIndex;
    public static int nTaskIndex;
    public static Task PreviousTask;

    public static Task CurrentTask
    {
        get {
			return (nCardIndex < cards.Length && nTaskIndex < CurrentCard.tasks.Length) ? cards[nCardIndex].tasks[nTaskIndex] : null;
		}
    }

    public static Card CurrentCard
    {
        get { return nCardIndex < cards.Length ? cards[nCardIndex] : null; }
    }

    void Awake () {
        cards = TextsBridge.GetCards();
        nCardsNumber = cards.Length;
        PreviousTask = CurrentTask;
        nCardIndex = 0;
        nTaskIndex = 0;
    }

    private void Start()
    {
        if(OnStartTasks != null) 
            OnStartTasks();
    }

    public static bool nextTask()
    {
        PreviousTask = CurrentTask;
        nTaskIndex++;
		bool isSameCard = true;
		if (nTaskIndex >= CurrentCard.tasks.Length)
		{
			nextCard();
			isSameCard = false;
		}
        if (OnTaskChanged != null)
            OnTaskChanged();
		return isSameCard;
    }

    public static void check()
    {
        CurrentTask.isAlreadySigned = true;
    }

    public static void nextCard()
    {
        nCardIndex++;
        nTaskIndex = 0;

        if (OnCardChanged != null)
            OnCardChanged();

        if (isFinished())
        {
            if (OnEndTasks != null)
            {

                OnEndTasks();
            }

            Debug.Log("Finished");
        }
            
    }

    public static bool isFinished()
    {
        return nCardIndex >= cards.Length;
    }
}
