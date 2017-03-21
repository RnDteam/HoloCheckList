﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class bigChecklistManager : MonoBehaviour {

    private enum TASK_STYLE
    {
        DESELECTED,
        SELECTED
    }

    private struct Task
    {
        public string strText;
        public bool bIsChecked;
    }

    private List<Card> Cards = new List<Card>();
    public List<Color> Colors = new List<Color>();
    public List<Color> TextColors = new List<Color>();
    public GameObject TaskPrefab;
    private float distanceBetweenTasks;
    private Vector3 position = Vector3.zero;
    public int numberOfRows;
    //public bool AutomaticMode = false;

    private List<GameObject> allTasks;
    private List<Task> lstTasks;
    private bool lastTaskComplete;
    public Text taskNumberText;
    public Text cardName;
    public VoiceManager voice;
    private GameObject taskParent;
    private Card CurrentCard;

    // Headers

    void Start () {
        distanceBetweenTasks = TaskPrefab.GetComponent<RectTransform>().rect.height;
        InitChecklist();
    }
    
    private void InitChecklist()
    {
        CurrentCard = TaskManager.CurrentCard;
        taskNumberText.text = string.Format("{0}/{1}", TaskManager.nTaskIndex + 1, CurrentCard.tasks.Length);
        cardName.text = string.Format("{0}", CurrentCard.name);

        if (taskParent != null)
        {
            Destroy(taskParent);
        }

        taskParent = new GameObject("TaskParent");
        taskParent.transform.parent = transform;
        taskParent.transform.localPosition = Vector3.zero;

        lastTaskComplete = false;
        lstTasks = new List<Task>();
        allTasks = new List<GameObject>();

        // Create task list
        for (int nTaskIndex = 0; nTaskIndex < CurrentCard.tasks.Length; nTaskIndex++)
        {
            Task t;
            t.bIsChecked = false;
            t.strText = CurrentCard.tasks[nTaskIndex].instruction;
            lstTasks.Add(t);
        }

        // Manage shown tasks
        for (int nTaskIndex = 0; nTaskIndex < CurrentCard.tasks.Length; nTaskIndex++)
        {
            allTasks.Add(CreateTask(nTaskIndex, "Task" + (nTaskIndex + 1), Colors[(int)TASK_STYLE.DESELECTED], TextColors[(int)TASK_STYLE.DESELECTED]));
            allTasks[nTaskIndex].transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = CurrentCard.tasks[nTaskIndex].instruction + " - " + nTaskIndex;

            if (nTaskIndex >= numberOfRows)
            {
                allTasks[nTaskIndex].SetActive(false);
            }
        }

        if(TaskManager.nCardIndex > 0)
        {
            Placed();
        }
    }

    #region place/move
    public void Placed()
    {
        ChangeColor(allTasks[TaskManager.nTaskIndex], Colors[(int)TASK_STYLE.SELECTED], TextColors[(int)TASK_STYLE.SELECTED]);
    }

    public void Moved()
    {
        ChangeColor(allTasks[TaskManager.nTaskIndex], Colors[(int)TASK_STYLE.DESELECTED], TextColors[(int)TASK_STYLE.DESELECTED]);
    }
    #endregion

    #region Create Tasks
    public GameObject CreateTask(int nTaskIndex, string name, Color backColor, Color textColor)
    {
        GameObject goTask = Instantiate(TaskPrefab, taskParent.transform);
        goTask.transform.FindChild("Toggle").FindChild("Label").GetComponent<Text>().text = TaskManager.nTaskIndex < CurrentCard.tasks.Length - 1 ? CurrentCard.tasks[TaskManager.nTaskIndex + 1].instruction : string.Empty;

        goTask.name = "Task" + nTaskIndex;
        goTask.transform.localScale = Vector3.one;
        ChangeColor(goTask, backColor, textColor);

        goTask.transform.localPosition = new Vector3(0, -nTaskIndex * distanceBetweenTasks, 0);
        goTask.transform.localRotation = Quaternion.identity;
        goTask.AddComponent<InteractableTask>();

        return goTask;
    }
    #endregion

    #region style changes
    public void ChangeColor(GameObject gameobject, Color backColor, Color textColor)
    {
        gameobject.transform.FindChild("Background").GetComponent<Image>().color = backColor;
        gameobject.transform.FindChild("Toggle").FindChild("Background").GetComponent<Image>().color = backColor;
        gameobject.transform.FindChild("Toggle").FindChild("Background").FindChild("Checkmark").GetComponent<Image>().color = textColor;
        gameobject.transform.FindChild("Toggle").FindChild("Label").GetComponent<Text>().color = textColor;
    }

    private void MarkTask(GameObject goTask, bool bIsChecked)
    {
        goTask.transform.FindChild("Toggle").GetComponent<Toggle>().isOn = bIsChecked;
    }
    #endregion

    #region Check/Next
    public void Check()
    {
        if (!GetComponent<PlaceableObject>().isPlaced) return;

        if (TaskManager.nTaskIndex < CurrentCard.tasks.Length)
        {
            // Checking if task need to be signed
            if(CurrentCard.tasks[TaskManager.nTaskIndex].signedTask)
            {
                taskNumberText.text = string.Format("{0}/{1}", TaskManager.nTaskIndex + 1, CurrentCard.tasks.Length);
                StartCoroutine(CheckAnimation());
                StartCoroutine(playCheckSound());
            }
            else
            {
                Debug.Log("Not signed task");
            }
        }
    }

    public void Next()
    {
        if (!GetComponent<PlaceableObject>().isPlaced) return;

        if (TaskManager.nTaskIndex < CurrentCard.tasks.Length)
        {
            taskNumberText.text = string.Format("{0}/{1}", TaskManager.nTaskIndex + 1, CurrentCard.tasks.Length);
            StartCoroutine(NextAnimation());
            StartCoroutine(playCheckSound());
        }
    }

    private IEnumerator CheckAnimation()
    {
        MarkTask(allTasks[TaskManager.nTaskIndex], true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(NextAnimation());
    }

    private IEnumerator NextAnimation()
    {
        // Change tasks colors
        ChangeColor(allTasks[TaskManager.nTaskIndex], Colors[(int)TASK_STYLE.DESELECTED], TextColors[(int)TASK_STYLE.DESELECTED]);
        TaskManager.nextTask();
        // Select next task, if card is over, switch cards
        if(TaskManager.nTaskIndex < allTasks.Count)
        {
            ChangeColor(allTasks[TaskManager.nTaskIndex], Colors[(int)TASK_STYLE.SELECTED], TextColors[(int)TASK_STYLE.SELECTED]);

            // Change displayed tasks
            allTasks[TaskManager.nTaskIndex - 1].SetActive(false);
            if (TaskManager.nTaskIndex + numberOfRows - 1 < allTasks.Count) allTasks[TaskManager.nTaskIndex + numberOfRows - 1].SetActive(true);
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(ScrollAnimation());
        }
        else
        {
            TaskManager.nextCard();
            InitChecklist();
        }
    }

    private IEnumerator ScrollAnimation()
    {
        for (int i = 0; i < distanceBetweenTasks; i++)
        {
            taskParent.transform.position += new Vector3(0, 1, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }
    public void Uncheck()
    {
        if(0 < TaskManager.nTaskIndex)
        { 
            TaskManager.nTaskIndex--;
            MarkTask(allTasks[TaskManager.nTaskIndex], false);
            StartCoroutine(playCheckSound());
            taskNumberText.text = string.Format("{0}/{1}", TaskManager.nTaskIndex + 1, CurrentCard.tasks.Length);
        }
    }
    #endregion
    
    #region Sound
    IEnumerator playCheckSound()
    {
        voice.StopAll();
        GetComponent<AudioSource>().Play();

        //if (AutomaticMode)
        //{
            yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length - 1);
            // TODO use it after adding relevant records
            //SayCurrentTask();
        //}
    }

    public void SayCurrentTask()
    {
        Debug.Log("say task");
        voice.StopAll();

        if (lastTaskComplete)
        {
            voice.Sounds[voice.Sounds.Length - 1].Play();
        }
        else
        {
            voice.Sounds[TaskManager.nTaskIndex].Play();
        }
    }
    #endregion
}