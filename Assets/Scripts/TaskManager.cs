using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TaskChange();

public class TaskManager : MonoBehaviour {

    public static event TaskChange OnStartTasks;
    public static event TaskChange OnTaskChanged;
    public static event TaskChange OnEndTasks;

    private static Card[] cards;
    public static int nCardIndex;
    public static int nTaskIndex;

    public static Task CurrentTask
    {
        get { return (nCardIndex < cards.Length && nTaskIndex < CurrentCard.tasks.Length) ? cards[nCardIndex].tasks[nTaskIndex] : null; }
    }

    public static Card CurrentCard
    {
        get { return nCardIndex < cards.Length ? cards[nCardIndex] : null; }
    }

    void Awake () {
        cards = TextsBridge.GetCards();
        nCardIndex = 0;
        nTaskIndex = 0;
    }

    private void Start()
    {
        if(OnStartTasks != null) 
            OnStartTasks();
    }

    public static void nextTask()
    {
        nTaskIndex++;

        if (OnTaskChanged != null)
            OnTaskChanged();
    }

    public static void nextCard()
    {
        nCardIndex++;
        nTaskIndex = 0;
        
        if (isFinished())
            if (OnEndTasks != null)
                OnEndTasks();
    }

    public static bool isFinished()
    {
        return nCardIndex >= cards.Length;
    }
}
