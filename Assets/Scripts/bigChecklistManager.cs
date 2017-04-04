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

    private bool lastTaskComplete;
    public VoiceManager voice;
	public PlaceableObject placeableObject;

	public TasksCard cardPrefab;
	public Transform cardsParent;
	private TasksCard[] displayedCards;
	private int CardsNumber;

    void Start () {
        //distanceBetweenTasks = TaskPrefab.GetComponent<RectTransform>().rect.height;
        //InitChecklist();

        //TaskManager.OnEndTasks += OnEndTasks;

		CreateCards();
    }

	void OnEnable()
	{
		TaskManager.OnEndTasks += OnEndTasks;
		TaskManager.OnTaskChanged += OnTaskChanged;
		TaskManager.OnCardChanged += OnCardChanged;
	}

	void OnDisable()
	{
		TaskManager.OnEndTasks -= OnEndTasks;
		TaskManager.OnTaskChanged -= OnTaskChanged;
		TaskManager.OnCardChanged -= OnCardChanged;
	}

    void OnEndTasks()
    {
        SceneManager.LoadScene("end-scene");
    }

	void OnTaskChanged()
	{
		foreach (var card in displayedCards)
		{
			card.AnimateTask();
		}
	}

	void OnCardChanged()
	{
		foreach (var card in displayedCards)
		{
			card.AnimateCard();
		}
	}

	private void CreateCards()
	{
		CardsNumber = TaskManager.CardsNumber;
		displayedCards = new TasksCard[CardsNumber];

		for(int nCardIndex = 0; nCardIndex < CardsNumber; nCardIndex++)
		{
			displayedCards[nCardIndex] = Instantiate(cardPrefab, cardsParent, true);
            displayedCards[nCardIndex].name = "card" + nCardIndex.ToString();
            if (nCardIndex != 0)
                displayedCards[nCardIndex].SetActiveBackground(false);
            //displayedCards[nCardIndex].transform.localPosition = cardPrefab.transform.position * (nCardIndex + 1);
            //displayedCards[nCardIndex].transform.localScale = cardPrefab.transform.localScale;
            displayedCards[nCardIndex].transform.SetAsFirstSibling();
			displayedCards[nCardIndex].SetCard(nCardIndex);
			//displayedCards[nCardIndex].GetComponent<SpriteRenderer>().sortingOrder = CardsNumber - nCardIndex - 1;
		}
	}

    /*private void InitChecklist()
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
		taskParent.transform.parent = tasksMask;
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

            if(TaskIndex >= TaskManager.TaskIndex && TaskIndex < TaskManager.TaskIndex + numberOfRows)
            {
                allTasks[TaskIndex].SetActive(true);
            } else
            {
                //allTasks[TaskIndex].SetActive(false);
            }
        }

        taskParent.transform.localPosition += new Vector3(0, distanceBetweenTasks * TaskManager.TaskIndex);

        // not placing in init function only when creating the tasks list at the first time
        if(TaskManager.CardIndex > 0 || TaskManager.TaskIndex > 0)
        {
            Placed();
        } else
        {
            if (OnMoved != null) OnMoved();
        }
    }*/

    #region place/move
    public void Placed()
    {
        if (OnPlaced != null) OnPlaced();
        //ChangeColor(allTasks[TaskManager.TaskIndex], TASK_STYLE.SELECTED, TaskManager.TaskIndex);
		foreach (var card in displayedCards)
		{
			card.AnimateTask();
		}
    }

    public void Moved()
    {
        if (OnMoved != null) OnMoved();
		foreach (var card in displayedCards)
		{
			card.DisableAllTasks();
		}
        //ChangeColor(allTasks[TaskManager.TaskIndex], TASK_STYLE.DESELECTED, TaskManager.TaskIndex);
    }
    #endregion

    #region Create Tasks
    /*public GameObject CreateTask(int TaskIndex, string name, TASK_STYLE tStyle)
    {
        GameObject goTask = Instantiate(TaskPrefab, taskParent.transform);
        goTask.transform.FindChild("Number").FindChild("Text").GetComponent<Text>().text = (TaskIndex + 1).ToString();

        goTask.name = "Task" + TaskIndex;
        goTask.transform.localScale = Vector3.one;
        ChangeColor(goTask, tStyle, TaskIndex);

        goTask.transform.localPosition = new Vector3(0, -TaskIndex * distanceBetweenTasks * (float) Math.Cos(gameObject.transform.rotation.eulerAngles.x * Math.PI/180), -TaskIndex * distanceBetweenTasks * (float)Math.Sin(gameObject.transform.rotation.eulerAngles.x * Math.PI / 180));
        goTask.transform.rotation = gameObject.transform.rotation;
        goTask.AddComponent<InteractableTask>();

        return goTask;
    }*/
    #endregion

    #region style changes
    /*private void ChangeColor(GameObject goTask, TASK_STYLE tStyle, int nTaskIndex)
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
    }*/
    #endregion

    #region Check/Next/Prev
    public void Check()
    {
		if (OneTaskController.Instance.IsChangingCard())
		{
			return;
		}

        int CurTaskIndex = TaskManager.TaskIndex;

		if (!placeableObject.isPlaced || TaskManager.CurrentCard == null) return;

		if (CurTaskIndex < TaskManager.CurrentCard.tasks.Length)
        {
            // Checking if task need to be signed
			if(TaskManager.CurrentCard.tasks[CurTaskIndex].signedTask)
            {
                TaskManager.check();
				displayedCards[TaskManager.CardIndex].MarkTask(CurTaskIndex, true);
                //taskNumberText.text = string.Format("{0}/{1}", CurTaskIndex + 1, CurrentCard.tasks.Length);
                //StartCoroutine(CheckAnimation(CurTaskIndex));
				Next();
            }
            else
            {
                Debug.Log("Not signed task");
            }
        }
    }

    public void Next()
    {
		if (OneTaskController.Instance.IsChangingCard())
		{
			return;
		}
		if (!placeableObject.isPlaced || TaskManager.CurrentCard == null) return;

		if (TaskManager.TaskIndex < TaskManager.CurrentCard.tasks.Length)
        {
            /*StartCoroutine(NextAnimation(false, TaskManager.TaskIndex));*/
			TaskManager.nextTask();
            StartCoroutine(playCheckSound());
        }
    }

    /*private IEnumerator CheckAnimation(int nTaskIndex)
    {
        MarkTask(allTasks[nTaskIndex], true);
        nTaskIndex = TaskManager.nextTask();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(NextAnimation(true, nTaskIndex));
    }*/

    /*
     * NextAnimation get a boolean variable - isChecked
     * 
     * if isChecked is true, NextAnimation was called from CheckAnimation, then NextAnimation dosen't in charge
     *  of notifying TaskManager of changing tasks.
     * 
     * 
     */
    /*private IEnumerator NextAnimation(bool isChecked, int nTaskIndex)
    {
        // Change tasks colors
        ChangeColor(allTasks[nTaskIndex], TASK_STYLE.DESELECTED, nTaskIndex);
        
        if (!isChecked) nTaskIndex = TaskManager.nextTask();

        Debug.Log("In bigchecklist " + nTaskIndex);
        // Select next task, if card is over, switch cards
		if(nTaskIndex > 0)
        {
            ChangeColor(allTasks[nTaskIndex], TASK_STYLE.SELECTED, nTaskIndex);

            // Change displayed tasks
            //allTasks[nTaskIndex - 1].SetActive(false);
            if (nTaskIndex + numberOfRows - 1 < allTasks.Count) allTasks[nTaskIndex + numberOfRows - 1].SetActive(true);
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(ScrollAnimation(true));
        }
        else
        {
            InitChecklist();
        }
    }*/

    /*private IEnumerator PrevAnimation(int nTaskIndex)
    {
        StartCoroutine(ScrollAnimation(false));
        yield return new WaitForSeconds(1f);

        taskNumberText.text = string.Format("{0}/{1}", nTaskIndex + 1, CurrentCard.tasks.Length);

        // Change tasks colors
        ChangeColor(allTasks[nTaskIndex + 1], TASK_STYLE.DESELECTED, nTaskIndex + 1);
        ChangeColor(allTasks[nTaskIndex], TASK_STYLE.SELECTED, nTaskIndex);

        // Change displayed tasks
        allTasks[nTaskIndex].SetActive(true);
        //if (nTaskIndex + numberOfRows < allTasks.Count) allTasks[nTaskIndex + numberOfRows].SetActive(false);
    }*/

    /*private IEnumerator ScrollAnimation(bool isNext)
    {
        for (int i = 0; i < distanceBetweenTasks; i++)
        {
			taskParent.transform.localPosition += new Vector3(0, isNext ? 1 : -1, 0);
            yield return new WaitForSeconds(delayAnimation);
        }
    }*/

    public void Undo()
    {
        if (0 < TaskManager.TaskIndex)
        {
			if (OneTaskController.Instance.IsChangingCard())
			{
				OneTaskController.Instance.CancelChangingCard();
			}
            int nTaskIdnex = TaskManager.prevTask();
            /*MarkTask(allTasks[nTaskIdnex], false);

            StartCoroutine(PrevAnimation(nTaskIdnex));*/

			displayedCards[TaskManager.CardIndex].MarkTask(nTaskIdnex, false);
        }
        else if(TaskManager.CardIndex > 0)
        {
			if (OneTaskController.Instance.IsChangingCard())
			{
				OneTaskController.Instance.CancelChangingCard();
			}
            TaskManager.prevTask();
            //InitChecklist();
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
