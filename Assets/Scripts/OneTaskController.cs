using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneTaskController : MonoBehaviour {

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

	void OnEnable()
	{
		TaskManager.OnStartTasks += StartTasks;
		TaskManager.OnEndTasks += EndTasks;
		TaskManager.OnTaskChanged += ChangeTask;
	}

	void OnDisable()
	{
		TaskManager.OnStartTasks -= StartTasks;
		TaskManager.OnEndTasks -= EndTasks;
		TaskManager.OnTaskChanged -= ChangeTask;
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

	void ChangeTask()
	{
		if (TaskManager.CurrentTask == null)
		{
			HideTask();
		}
		else
		{
			SetTask();
		}
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
		TaskStrips[0].SetValidated(true);

		updateSize = true;

		animate = true;
		index = 0;
		rate = 1.0f/enterSpeed;
	}

	void Update()
	{
		if (animate)
		{
			if(index<1.0f)
			{
				if(index == 0)
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
				index+=rate*Time.deltaTime;
			}else{
				animate=false;
				TaskStripsRectTransform[0].anchoredPosition3D = inEndPos;
				TaskStripsRectTransform[1].anchoredPosition3D = outEndPos;
				index=1.0f;
			}
		}
	}

	void ShowTask()
	{
		if (TaskManager.CurrentTask != null)
		{
			TaskParent.SetActive(true);
		}
	}

	void HideTask()
	{
		TaskParent.SetActive(false);
	}
}
