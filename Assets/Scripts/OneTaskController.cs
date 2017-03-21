using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTaskController : MonoBehaviour {

	public GameObject TaskParent;
	public TaskStrip TaskStrip;

	void OnEnable()
	{
		// TODO: Register to Tasks events
	}

	void OnDisable()
	{
		// TODO: Unregister from Tasks events
	}

	void SetTask()
	{
		TaskStrip.SetTaskText("New Task");
		TaskStrip.ShowInfoIcon(true);
		TaskStrip.ShowValidationIcon(true);
		TaskStrip.SetValidated(true);
	}

	void ShowTask()
	{
		TaskParent.SetActive(true);
		SetTask();
	}

	void HideTask()
	{
		TaskParent.SetActive(false);
	}
}
