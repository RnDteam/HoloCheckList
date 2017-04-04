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
			return (cards != null && CardIndex < cards.Length && TaskIndex < CurrentCard.tasks.Length) ? cards[CardIndex].tasks[TaskIndex] : null;
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
        asgo = new GameObject("RecordController");
        audSource = asgo.AddComponent<AudioSource>();
        asgo.transform.position = Vector3.zero;
    }

    public static void InitTaskManager()
    {
        cards = TextsBridge.GetCards();

        if (cards != null)
        {
            PreviousTask = CurrentTask;

            PrepareTaskParameters();
            CardsNumber = cards.Length;
            PreviousTask = CurrentTask;
            CardIndex = 0;
            TaskIndex = 0;
        }
    }

    private void Start()
    {
        StartCoroutine(PlayCoroutine("haklatot\\other\\say-place"));
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
                changeCard(isNext);
            }
            else
            {
                TaskIndex += 1;
            }
        } else // previous task
        {
            if(TaskIndex == 0 && CardIndex > 0) // Goes to previous card 
            {
                changeCard(isNext);
            } else if(TaskIndex > 0)
            {
                TaskIndex -= 1;
            }

            CurrentTask.isAlreadySigned = false;
        }
        if (CurrentTask != null && CurrentTask.file != null)
        {
            if (TaskIndex == 0)
                instance.StartCoroutine(instance.PlayCoroutine("haklatot\\" + CurrentCard.folder + "\\" + CurrentTask.file, false));
            else
                instance.StartCoroutine(instance.PlayCoroutine("haklatot\\" + CurrentCard.folder + "\\" + CurrentTask.file));
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

    public IEnumerator PlayCoroutine(string file, bool interrupt = true)
    {
        while (!interrupt && audSource.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(PlaySound(file));
    }

    private IEnumerator PlayDone()
    {
        while (audSource.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return PlayCoroutine("haklatot\\other\\done", false);
        while (audSource.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }
        OnEndTasks();
        yield return null;
    }

    public static void changeCard(bool isNext)
    {
        if (CurrentCard.finish != null && CurrentCard.tasks.Length - 1 == TaskIndex)
        {
            instance.StartCoroutine(instance.PlayCoroutine("haklatot\\" + CurrentCard.folder + "\\" + CurrentCard.finish, false));
        }
        CardIndex += isNext ? 1 : -1;
        TaskIndex = isNext ? 0 : CurrentCard.tasks.Length - 1;

        if (isFinished())
		{
            instance.StartCoroutine(instance.PlayDone());
		}
		else
		{
			if (OnCardChanged != null)
				OnCardChanged();
		}
        if (CurrentCard != null && CurrentCard.start != null && TaskIndex == 0)
        {
            instance.StartCoroutine(instance.PlayCoroutine("haklatot\\" + CurrentCard.folder + "\\" + CurrentCard.start, false));
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
