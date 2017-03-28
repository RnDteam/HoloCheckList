using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public delegate void BigChecklistStateChanged();

public class bigChecklistManager : MonoBehaviour {

    public static event BigChecklistStateChanged OnPlaced;
    public static event BigChecklistStateChanged OnMoved;
    public float delayAnimation;

    public enum TASK_STYLE
    {
        DESELECTED,
        SELECTED
    }

    private struct Task
    {
        public string strText;
        public bool bIsChecked;
    }

    public Sprite[] validation;
    public Sprite[] info;
    public Sprite validated;

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
	public PlaceableObject placeableObject;

    void Start () {
        distanceBetweenTasks = TaskPrefab.GetComponent<RectTransform>().rect.height;
        InitChecklist();

        TaskManager.OnEndTasks += OnEndTasks;
    }

    void OnEndTasks()
    {
        SceneManager.LoadScene("end-scene");
    }

    private void OnDestroy()
    {
        TaskManager.OnEndTasks -= OnEndTasks;
    }

    private void InitChecklist()
    {
        CurrentCard = TaskManager.CurrentCard;
		if (CurrentCard == null)
		{
			return;
		}
        taskNumberText.text = string.Format("{0}/{1}", TaskManager.TaskIndex + 1, CurrentCard.tasks.Length);
        cardName.text = string.Format("{0}", CurrentCard.name);

        if (taskParent != null)
        {
            Destroy(taskParent);
        }

        taskParent = new GameObject("TaskParent");
        taskParent.transform.parent = transform;
        taskParent.transform.localPosition = Vector3.zero;
		taskParent.transform.localScale = Vector3.one;

        lastTaskComplete = false;
        lstTasks = new List<Task>();
        allTasks = new List<GameObject>();

        // Create task list
        for (int TaskIndex = 0; TaskIndex < CurrentCard.tasks.Length; TaskIndex++)
        {
            Task t;
            t.bIsChecked = false;
            t.strText = CurrentCard.tasks[TaskIndex].instruction;
            lstTasks.Add(t);
        }

        // Manage shown tasks
        for (int TaskIndex = 0; TaskIndex < CurrentCard.tasks.Length; TaskIndex++)
        {
            allTasks.Add(CreateTask(TaskIndex, "Task" + (TaskIndex + 1), TASK_STYLE.DESELECTED));
            allTasks[TaskIndex].transform.FindChild("Task").Find("instruction").GetComponent<Text>().text = CurrentCard.tasks[TaskIndex].instruction;

            if (TaskIndex >= numberOfRows)
            {
                allTasks[TaskIndex].SetActive(false);
            }
        }

        if(TaskManager.CardIndex > 0)
        {
            Placed();
        } else
        {
            if (OnMoved != null) OnMoved();
        }
    }

    #region place/move
    public void Placed()
    {
        if (OnPlaced != null) OnPlaced();

        ChangeColor(allTasks[TaskManager.TaskIndex], TASK_STYLE.SELECTED, TaskManager.TaskIndex);
    }

    public void Moved()
    {
        if (OnMoved != null) OnMoved();

        ChangeColor(allTasks[TaskManager.TaskIndex], TASK_STYLE.DESELECTED, TaskManager.TaskIndex);
    }
    #endregion

    #region Create Tasks
    public GameObject CreateTask(int TaskIndex, string name, TASK_STYLE tStyle)
    {
        GameObject goTask = Instantiate(TaskPrefab, taskParent.transform);
        goTask.transform.FindChild("Number").FindChild("Text").GetComponent<Text>().text = (TaskIndex + 1).ToString();

        goTask.name = "Task" + TaskIndex;
        goTask.transform.localScale = Vector3.one;
        ChangeColor(goTask, tStyle, TaskIndex);

        goTask.transform.localPosition = new Vector3(0, -TaskIndex * distanceBetweenTasks, 0);
        goTask.transform.localRotation = Quaternion.identity;
        goTask.AddComponent<InteractableTask>();

        return goTask;
    }
    #endregion

    #region style changes
    private void ChangeColor(GameObject goTask, TASK_STYLE tStyle, int nTaskIndex)
    {
        if (CurrentCard != null)
        {
            Transform tTask = goTask.transform.FindChild("Task");
            Color backColor = Colors[(int)tStyle];
            Color textColor = TextColors[(int)tStyle];
            Sprite spInfo = info[(int)tStyle];
            Sprite spValidation = validation[(int)tStyle];

            goTask.transform.FindChild("Number").GetComponent<Image>().color = backColor;
            tTask.GetComponent<Image>().color = backColor;
            tTask.FindChild("ValidationIcon").gameObject.SetActive(CurrentCard.tasks[nTaskIndex].signedTask);
            tTask.FindChild("InfoIcon").gameObject.SetActive(!string.IsNullOrEmpty(CurrentCard.tasks[nTaskIndex].hasExtraInfo));
            tTask.FindChild("InfoIcon").gameObject.GetComponent<Image>().sprite = spInfo;
            tTask.FindChild("ValidationIcon").gameObject.GetComponent<Image>().sprite = spValidation;
        }
    }

    private void MarkTask(GameObject goTask, bool bIsChecked)
    {
        goTask.transform.FindChild("Task").FindChild("ValidationIcon").gameObject.GetComponent<Image>().sprite = validated;
    }
    #endregion

    #region Check/Next/Prev
    public void Check()
    {
        int CurTaskIndex = TaskManager.TaskIndex;

        if (!placeableObject.isPlaced || CurrentCard == null) return;

        if (CurTaskIndex < CurrentCard.tasks.Length)
        {
            // Checking if task need to be signed
            if(CurrentCard.tasks[CurTaskIndex].signedTask)
            {
                TaskManager.check();
                taskNumberText.text = string.Format("{0}/{1}", CurTaskIndex + 1, CurrentCard.tasks.Length);
                StartCoroutine(CheckAnimation(CurTaskIndex));
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
		if (!placeableObject.isPlaced || CurrentCard == null) return;

        if (TaskManager.TaskIndex < CurrentCard.tasks.Length)
        {
            taskNumberText.text = string.Format("{0}/{1}", TaskManager.TaskIndex + 1, CurrentCard.tasks.Length);
            StartCoroutine(NextAnimation(false, TaskManager.TaskIndex));
            StartCoroutine(playCheckSound());
        }
    }

    private IEnumerator CheckAnimation(int nTaskIndex)
    {
        MarkTask(allTasks[nTaskIndex], true);
        nTaskIndex = TaskManager.nextTask();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(NextAnimation(true, nTaskIndex));
    }

    /*
     * NextAnimation get a boolean variable - isChecked
     * 
     * if isChecked is true, NextAnimation was called from CheckAnimation, then NextAnimation dosen't in charge
     *  of notifying TaskManager of changing tasks.
     * 
     * 
     */
    private IEnumerator NextAnimation(bool isChecked, int nTaskIndex)
    {
        // Change tasks colors
        ChangeColor(allTasks[nTaskIndex], TASK_STYLE.DESELECTED, nTaskIndex);
        
        if (!isChecked) nTaskIndex = TaskManager.nextTask();

        // Select next task, if card is over, switch cards
		if(nTaskIndex > 0)
        {
            ChangeColor(allTasks[nTaskIndex], TASK_STYLE.SELECTED, nTaskIndex);

            // Change displayed tasks
            allTasks[nTaskIndex - 1].SetActive(false);
            if (nTaskIndex + numberOfRows - 1 < allTasks.Count) allTasks[nTaskIndex + numberOfRows - 1].SetActive(true);
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(ScrollAnimation(true));
        }
        else
        {
            InitChecklist();
        }
    }

    private IEnumerator PrevAnimation(int nTaskIndex)
    {
        StartCoroutine(ScrollAnimation(false));
        yield return new WaitForSeconds(1f);

        taskNumberText.text = string.Format("{0}/{1}", nTaskIndex + 1, CurrentCard.tasks.Length);

        // Change tasks colors
        ChangeColor(allTasks[nTaskIndex + 1], TASK_STYLE.DESELECTED, nTaskIndex + 1);
        ChangeColor(allTasks[nTaskIndex], TASK_STYLE.SELECTED, nTaskIndex);

        // Change displayed tasks
        allTasks[nTaskIndex].SetActive(true);
        if (nTaskIndex + numberOfRows < allTasks.Count) allTasks[nTaskIndex + numberOfRows].SetActive(false);
    }

    private IEnumerator ScrollAnimation(bool isNext)
    {
        for (int i = 0; i < distanceBetweenTasks; i++)
        {
			taskParent.transform.localPosition += new Vector3(0, isNext ? 1 : -1, 0);
            yield return new WaitForSeconds(delayAnimation);
        }
    }

    public void Undo()
    {
        if (0 < TaskManager.TaskIndex)
        {
            int nTaskIdnex = TaskManager.prevTask();
            MarkTask(allTasks[nTaskIdnex], false);

            StartCoroutine(PrevAnimation(nTaskIdnex));
        }
        else if(TaskManager.CardIndex > 0)
        {
            TaskManager.prevTask();
            InitChecklist();
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
            voice.Sounds[TaskManager.TaskIndex].Play();
        }
    }
    #endregion
}
