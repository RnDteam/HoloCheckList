using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTaskController : MonoBehaviour {

	public GameObject TaskParent;
	public TaskStrip TaskStrip;

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
