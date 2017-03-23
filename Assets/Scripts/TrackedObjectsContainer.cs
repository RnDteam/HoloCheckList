using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedObjectsContainer : MonoBehaviour {

	public static TrackedObjectsContainer Instance;

	private Dictionary<string, GameObject> trackedObjects = new Dictionary<string, GameObject>();

	private string currentTrackedObjectName = "";

	// Use this for initialization
	void Awake () {
		Instance = this;
		foreach(Transform trackedObject in transform)
		{
			trackedObjects.Add(trackedObject.name, trackedObject.gameObject);
		}
		DisableAllTrackedObjects();
	}

    private void Start()
    {
        CheckCurrentTask();
    }

    private void OnEnable()
    {
        TaskManager.OnTaskChanged += CheckCurrentTask;
    }

    private void OnDisable()
    {
        TaskManager.OnTaskChanged -= CheckCurrentTask;
    }

    void CheckCurrentTask()
    {
		string extraInfoName = "";
		if (TaskManager.CurrentTask != null)
		{
			extraInfoName = TaskManager.CurrentTask.hasExtraInfo;
		}
		SetCurrentTrackedObject(extraInfoName);
    }

    public void DisableAllTrackedObjects()
	{
		foreach (var trackedObject in trackedObjects)
		{
			trackedObject.Value.SetActive(false);
		}
	}

	public void EnableCurrentTrackedObject()
	{
		if (trackedObjects.ContainsKey(currentTrackedObjectName))
		{
			trackedObjects[currentTrackedObjectName].SetActive(true);
		}
	}

	public void DisableCurrentTrackedObject()
	{
		if (trackedObjects.ContainsKey(currentTrackedObjectName))
		{
			trackedObjects[currentTrackedObjectName].SetActive(false);
		}
	}

	public void SetCurrentTrackedObject(string objectName)
	{
		if (currentTrackedObjectName != objectName)
		{
			DisableCurrentTrackedObject();
			currentTrackedObjectName = objectName;
			EnableCurrentTrackedObject();
		}
	}
	
}