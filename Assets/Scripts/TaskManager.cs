using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void TaskChange();

public class TaskManager : MonoBehaviour {

    private static TaskManager instance;

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
    private static AudioSource audSource;
    private static GameObject asgo;

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

    void Awake () {
        InitTaskManager();
        asgo = new GameObject();
        audSource = asgo.AddComponent<AudioSource>();
        asgo.transform.position = Vector3.zero;
        instance = this;
    }

    private static void InitTaskManager()
    {
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

    public static void SomethingWasPlaced()
    {
        if (CurrentTask != null && CurrentTask.file != null)
        {
            PlaySound("haklatot\\" + CurrentCard.folder + "\\" + CurrentTask.file);
        }
    }

    public static int changeTask(bool isNext)
    {
        PreviousTask = CurrentTask;
        
        if(isNext)
        {
            if (TaskIndex >= CurrentCard.tasks.Length - 1) // Goes to next card 
            {
                instance.StartCoroutine(changeCard(isNext));
            }
            else
            {
                TaskIndex += 1;
            }
        } else // previous task
        {
            if(TaskIndex == 0 && CardIndex > 0) // Goes to previous card 
            {
                instance.StartCoroutine(changeCard(isNext));
            } else if(TaskIndex > 0)
            {
                TaskIndex -= 1;
            }

            CurrentTask.isAlreadySigned = false;
        }
        if (CurrentTask != null && CurrentTask.file != null)
        {
            PlaySound("haklatot\\" + CurrentCard.folder + "\\" + CurrentTask.file);
        }

        if (OnTaskChanged != null)
            OnTaskChanged();

        return TaskIndex;
    }


    public static void check()
    {
        CurrentTask.isAlreadySigned = true;
    }

    private static float PlaySound(string file)
    {
        audSource.Stop();
        audSource.clip = (AudioClip)Resources.Load(file);
        audSource.Play();
        return audSource.clip.length;
    }

    public static IEnumerator changeCard(bool isNext)
    {
        if (CurrentCard.finish != null)
        {
            yield return new WaitForSeconds(PlaySound("haklatot\\" + CurrentCard.folder + "\\" + CurrentCard.finish));
        }
        CardIndex += isNext ? 1 : -1;
        TaskIndex = isNext ? 1 : CurrentCard.tasks.Length - 2;

        if (OnCardChanged != null)
            OnCardChanged();

        if (isFinished())
            if (OnEndTasks != null)
                OnEndTasks();
        if (CurrentCard != null && CurrentCard.start != null)
        {
            Debug.Log("EndCard");
            yield return new WaitForSeconds(PlaySound("haklatot\\" + CurrentCard.folder + "\\" + CurrentCard.start));
        }
        
        if (CurrentCard != null)
        {
            Debug.Log("changeing task to" + TaskIndex);
            if (TaskIndex == 1)
                changeTask(false);
            else
                changeTask(true);
            Debug.Log("now task is " + TaskIndex);
        }
        yield return null;
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
