using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneTaskController : MonoBehaviour {

	public GameObject TaskParent;
	public TaskStrip TaskStrip;
	public ContentSizeFitter sizeFitter;
	private bool updateSize = false;

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
		ShowTask();
		ChangeTask();
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
		Task task = TaskManager.CurrentTask;
		TaskStrip.SetTaskText(task.instruction);
		TaskStrip.ShowInfoIcon(!string.IsNullOrEmpty(task.hasExtraInfo));
		TaskStrip.ShowValidationIcon(task.signedTask);
		TaskStrip.SetValidated(true);
		updateSize = true;
	}

	void Update()
	{
		if (updateSize)
		{
			updateSize = false;
			sizeFitter.enabled = false;
			sizeFitter.enabled = true;
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
