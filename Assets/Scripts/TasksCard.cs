using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TasksCard : MonoBehaviour {

	private struct Task
	{
		public string strText;
		public bool bIsChecked;
	}

	public enum TASK_STYLE
	{
		DESELECTED,
		SELECTED
	}

	public int CardNumber;

	public Text CardName;
	public Transform TasksArea;

	public Vector3 BasePosition;
	public Vector3 OutPosition;

	public GameObject TaskPrefab;
	public List<Color> Colors = new List<Color>();
	public List<Color> TextColors = new List<Color>();
	public Sprite[] validation;
	public Sprite[] info;
	public Sprite validated;


	private GameObject taskParent;

	private List<GameObject> allTasks;
	private List<Task> lstTasks;

	private Card CurrentCard;

	private float distanceBetweenTasks;

	private bool animateTasks=false;
	private bool animateCards=false;
	private float tasksAnimationIndex=0;
	private float cardsAnimationIndex=0;
	private float tasksAnimationRate;
	private float cardsAnimationRate;

	private Vector3 tasksStartPos;
	private Vector3 tasksEndPos;

	private Vector3 cardStartPos;
	private Vector3 cardEndPos;

	public float TasksAnimationSpeed= 1f;
	public float CardsAnimationSpeed= 1f;

	private bool disabledTasks = false;

	private int prevTask = 0;
	private bool enableTaskOnAnimationEnd = false;

	void Awake()
	{
		distanceBetweenTasks = TaskPrefab.GetComponent<RectTransform>().rect.height;
	}

	void Update()
	{
		if (animateTasks)
		{
			if (tasksAnimationIndex < 1.0f)
			{
				taskParent.transform.localPosition = Vector3.Lerp(tasksStartPos, tasksEndPos, tasksAnimationIndex);
				tasksAnimationIndex += tasksAnimationRate * Time.deltaTime;
			}
			else
			{
				animateTasks = false;
				taskParent.transform.localPosition = tasksEndPos;
				tasksAnimationIndex = 1.0f;
				if (enableTaskOnAnimationEnd)
				{
					EnableTask(prevTask);
				}
			}
		}
		if (animateCards)
		{
			if (cardsAnimationIndex < 1.0f)
			{
				transform.localPosition = Vector3.Lerp(cardStartPos, cardEndPos, cardsAnimationIndex);
				cardsAnimationIndex += cardsAnimationRate * Time.deltaTime;
			}
			else
			{
				animateCards = false;
				transform.localPosition = cardEndPos;
				cardsAnimationIndex = 1.0f;
			}
		}
	}

	public void SetCard(int cardNumber)
	{
		CurrentCard = TaskManager.GetCard(cardNumber);
		CardNumber = cardNumber;
		CardName.text = CurrentCard.name;
		transform.localPosition = BasePosition * (cardNumber+1);
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;

		taskParent = new GameObject("TaskParent");
		taskParent.transform.parent = TasksArea;
		taskParent.transform.localPosition = Vector3.zero;
		taskParent.transform.localScale = Vector3.one;

		lstTasks = new List<Task>();
		allTasks = new List<GameObject>();



		for (int TaskIndex = 0; TaskIndex < CurrentCard.tasks.Length; TaskIndex++)
		{
			Task t;
			t.bIsChecked = false;
			t.strText = CurrentCard.tasks[TaskIndex].instruction;
			lstTasks.Add(t);
		}

		for (int TaskIndex = 0; TaskIndex < CurrentCard.tasks.Length; TaskIndex++)
		{
			allTasks.Add(CreateTask(TaskIndex, "Task" + (TaskIndex + 1), TASK_STYLE.DESELECTED));
			allTasks[TaskIndex].transform.FindChild("Task").Find("instruction").GetComponent<Text>().text = CurrentCard.tasks[TaskIndex].instruction;
		}
	}

	public GameObject CreateTask(int TaskIndex, string name, TASK_STYLE tStyle)
	{
		GameObject goTask = Instantiate(TaskPrefab, taskParent.transform);
		goTask.transform.FindChild("Number").FindChild("Text").GetComponent<Text>().text = (TaskIndex + 1).ToString();

		goTask.name = "Task" + TaskIndex;
		goTask.transform.localScale = Vector3.one;
		ChangeColor(goTask, tStyle, TaskIndex);

		goTask.transform.localPosition = new Vector3(0, -TaskIndex * distanceBetweenTasks, 0);
		goTask.transform.rotation = gameObject.transform.rotation;
		goTask.AddComponent<InteractableTask>();

		return goTask;
	}

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
			if (CurrentCard.tasks[nTaskIndex].isAlreadySigned)
			{
				tTask.FindChild("ValidationIcon").gameObject.GetComponent<Image>().sprite = validated;
			}
			else
			{
				tTask.FindChild("ValidationIcon").gameObject.GetComponent<Image>().sprite = spValidation;
			}
		}
	}

	public void MarkTask(int tIndex, bool bIsChecked)
	{
		GameObject goTask = allTasks[tIndex];
		if (bIsChecked)
		{
			goTask.transform.FindChild("Task").FindChild("ValidationIcon").gameObject.GetComponent<Image>().sprite = validated;
		}
		else
		{
			Sprite spValidation = validation[(int)TASK_STYLE.DESELECTED];
			goTask.transform.FindChild("Task").FindChild("ValidationIcon").gameObject.GetComponent<Image>().sprite = spValidation;
		}
	}

	public void AnimateCard()
	{
		animateCards = true;
		cardsAnimationIndex = 0;
		cardsAnimationRate = 1.0f/CardsAnimationSpeed;
		cardStartPos = transform.localPosition;
		//cardEndPos;
		if (TaskManager.CardIndex == CardNumber)
		{
			cardEndPos = BasePosition;
		}
		else if (TaskManager.CardIndex < CardNumber)
		{
			cardEndPos = BasePosition * ( CardNumber - TaskManager.CardIndex + 1 );
		}
		else
		{
			cardEndPos = OutPosition;
		}
	}

	public void AnimateTask()
	{
		if (TaskManager.CardIndex != CardNumber)
		{
			if (!disabledTasks)
			{
				DisableAllTasks();
			}
			return;
		}
		disabledTasks = false;
		animateTasks = true;
		tasksAnimationIndex = 0;
		tasksAnimationRate = 1.0f/TasksAnimationSpeed;
		tasksStartPos = new Vector3(0, distanceBetweenTasks * prevTask, 0);
		tasksEndPos = new Vector3(0, distanceBetweenTasks * TaskManager.TaskIndex, 0);

		if (prevTask <= TaskManager.TaskIndex)
		{
			EnableTask(TaskManager.TaskIndex);
		}
		else
		{
			EnableTask(prevTask);
			enableTaskOnAnimationEnd = true;
		}

		prevTask = TaskManager.TaskIndex;
	}

	private void EnableTask(int tIndex)
	{
		if (TaskManager.CardIndex != CardNumber)
		{
			return;
		}
		ChangeColor(allTasks[tIndex], TASK_STYLE.SELECTED, tIndex);
		if (tIndex < allTasks.Count-1)
		{
			ChangeColor(allTasks[tIndex+1], TASK_STYLE.DESELECTED, tIndex+1);
		}
		if (tIndex > 0)
		{
			ChangeColor(allTasks[tIndex-1], TASK_STYLE.DESELECTED, tIndex-1);
		}
	}

	public void DisableAllTasks()
	{
		disabledTasks = true;
		for (int i=0; i<allTasks.Count; i++)
		{
			ChangeColor(allTasks[i], TASK_STYLE.DESELECTED, i);
		}
	}
}
