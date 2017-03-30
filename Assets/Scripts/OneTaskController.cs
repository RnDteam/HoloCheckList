using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneTaskController : MonoBehaviour {

	public static OneTaskController Instance;

	public GameObject TaskParent;
	public TaskStrip[] TaskStrips;
	private RectTransform[] TaskStripsRectTransform = new RectTransform[2];
	private bool updateSize = false;

	public Vector3 inStartPos;
	public Vector3 inEndPos;
	public Vector3 outStartPos;
	public Vector3 outEndPos;

	public float enterSpeed = 1f;

	private bool animate=false;
	private float index=0;
	private float rate;
    public float delay;
    private float curDelay;

	private bool isChangingCard = false;

	private int currentCardIndex = 0;

	public bool IsChangingCard()
	{
		return isChangingCard;
	}

	void Awake()
	{
		Instance = this;
	}

	void OnEnable()
	{
        bigChecklistManager.OnPlaced += StartTasks;
        bigChecklistManager.OnMoved += EndTasks;
        TaskManager.OnStartTasks += StartTasks;
		TaskManager.OnEndTasks += EndTasks;
		TaskManager.OnTaskChanged += ChangeTask;
		TaskManager.OnCardChanged += ChangeCard;
	}

	void OnDisable()
	{
        bigChecklistManager.OnPlaced -= StartTasks;
        bigChecklistManager.OnMoved -= EndTasks;
        TaskManager.OnStartTasks -= StartTasks;
		TaskManager.OnEndTasks -= EndTasks;
		TaskManager.OnTaskChanged -= ChangeTask;
		TaskManager.OnCardChanged -= ChangeCard;
	}

	void StartTasks()
	{
		ChangeTask();
		ShowTask();
	}

	void EndTasks()
	{
		HideTask();
	}

	void ChangeCard()
	{
		if (TaskManager.CardIndex > currentCardIndex)
		{
			isChangingCard = true;
			SetCard();
		}
		currentCardIndex = TaskManager.CardIndex;
	}

	void ChangeTask()
	{
		if (isChangingCard)
		{
			return;
		}
		if (TaskManager.CurrentTask == null)
		{
			HideTask();
		}
		else
		{
			SetTask();
		}
	}

	IEnumerator MoveToNextTaskFromCard()
	{
		yield return new WaitForSeconds(2f);
		isChangingCard = false;
		ChangeTask();
	}

	void SetCard()
	{
		if (!TaskParent.activeSelf)
		{
			TaskStrips[0].gameObject.SetActive(true);
			TaskStrips[1].gameObject.SetActive(false);
		}
		else
		{
			TaskStrip temp = TaskStrips[0];
			TaskStrips[0] = TaskStrips[1];
			TaskStrips[1] = temp;
			temp = null;
		}
		TaskStripsRectTransform[0] = TaskStrips[0].GetComponent<RectTransform>();
		TaskStripsRectTransform[1] = TaskStrips[1].GetComponent<RectTransform>();

		Card cardNew = TaskManager.CurrentCard;
		// TODO: get the old card from a merge
		Card cardOld = TaskManager.GetCard(currentCardIndex);
		string firstPart = TextsBridge.ReverseHebrewName("סיימת שלב ");
		string endPart = TextsBridge.ReverseHebrewName(", עבור לשלב ");
		TaskStrips[0].SetTaskText(cardNew.name + endPart + cardOld.name + firstPart);
		TaskStrips[0].ShowInfoIcon(false);
		TaskStrips[0].ShowValidationIcon(false);
		TaskStrips[0].SetValidated(false);
		TaskStrips[1].SetValidated(TaskManager.PreviousTask.isAlreadySigned);

		updateSize = true;

		animate = true;
		index = 0;
		rate = 1.0f/enterSpeed;
		curDelay = TaskManager.PreviousTask.isAlreadySigned ? delay : 0;
	}

	void SetTask()
	{
		if (!TaskParent.activeSelf)
		{
			TaskStrips[0].gameObject.SetActive(true);
			TaskStrips[1].gameObject.SetActive(false);
		}
		else
		{
			TaskStrip temp = TaskStrips[0];
			TaskStrips[0] = TaskStrips[1];
			TaskStrips[1] = temp;
			temp = null;
		}
		TaskStripsRectTransform[0] = TaskStrips[0].GetComponent<RectTransform>();
		TaskStripsRectTransform[1] = TaskStrips[1].GetComponent<RectTransform>();

		Task task = TaskManager.CurrentTask;
		TaskStrips[0].SetTaskText(task.instruction);
		TaskStrips[0].ShowInfoIcon(!string.IsNullOrEmpty(task.hasExtraInfo));
		TaskStrips[0].ShowValidationIcon(task.signedTask);
		TaskStrips[0].SetValidated(task.isAlreadySigned);
        TaskStrips[1].SetValidated(TaskManager.PreviousTask.isAlreadySigned);

        updateSize = true;

		animate = true;
		index = 0;
		rate = 1.0f/enterSpeed;
        curDelay = TaskManager.PreviousTask.isAlreadySigned ? delay : 0;
	}

	void Update()
	{
		if (animate)
		{
            if(curDelay <= 0)
            {
                if (index < 1.0f)
                {
                    if (index == 0)
                    {
                        TaskStrips[0].gameObject.SetActive(false);//SetTaskText(TaskStrips[0].TaskText.text);
                    }
                    else if (updateSize)
                    {
                        updateSize = false;
                        TaskStrips[0].gameObject.SetActive(true);
                    }
                    TaskStripsRectTransform[0].anchoredPosition3D = Vector3.Lerp(inStartPos, inEndPos, index);
                    TaskStripsRectTransform[1].anchoredPosition3D = Vector3.Lerp(outStartPos, outEndPos, index);
                    index += rate * Time.deltaTime;
                }
                else
                {
                    animate = false;
                    TaskStripsRectTransform[0].anchoredPosition3D = inEndPos;
                    TaskStripsRectTransform[1].anchoredPosition3D = outEndPos;
                    index = 1.0f;
					if (isChangingCard)
					{
						StartCoroutine(MoveToNextTaskFromCard());
					}
                }
            } else
            {
                curDelay -= 0.01f;
            }
		}
	}

	public void ShowTask()
	{
		if (TaskManager.CurrentTask != null)
		{
			TaskParent.SetActive(true);
		}
	}

	public void HideTask()
	{
		TaskParent.SetActive(false);
	}
}
