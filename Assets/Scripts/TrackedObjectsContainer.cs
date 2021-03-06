﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackedObjectsContainer : MonoBehaviour {

	public static TrackedObjectsContainer Instance;

	private Dictionary<string, GameObject> trackedObjects = new Dictionary<string, GameObject>();

	private string currentTrackedObjectName = "";

	private bool vuforiaStartCalled = false;
    
	void Awake () {
		Instance = this;
		foreach(Transform trackedObject in transform)
		{
			trackedObjects.Add(trackedObject.name, trackedObject.gameObject);
		}
	}

    private void Start()
    {
		DisableAllTrackedObjects();
    }

    private void OnEnable()
    {
        TaskManager.OnTaskChanged += CheckCurrentTask;
    }

    private void OnDisable()
    {
        TaskManager.OnTaskChanged -= CheckCurrentTask;
    }

	void StartVuforia()
	{
		if (!vuforiaStartCalled)
		{
			vuforiaStartCalled = true;
			VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadDataSet);
		}
	}

	void LoadDataSet()
	{
		VuforiaARController.Instance.UnregisterVuforiaStartedCallback(LoadDataSet);
		string dataSetName = "HoloC";
		ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
		         
		DataSet dataSet = objectTracker.CreateDataSet();
		         
		EnableAllTrackedObjects();

		if (dataSet.Load(dataSetName)) {
			             
			objectTracker.Stop();  // stop tracker so that we can add new dataset
			 
			if (!objectTracker.ActivateDataSet(dataSet)) {
				// Note: ImageTracker cannot have more than 100 total targets activated
				Debug.Log("<color=yellow>Failed to Activate DataSet: " + dataSetName + "</color>");
			}
			 
			if (!objectTracker.Start()) {
				Debug.Log("<color=yellow>Tracker Failed to Start.</color>");
			}

			DisableAllTrackedObjects();
			EnableCurrentTrackedObject();
		}
	}

    public void CheckCurrentTask()
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

	public void EnableAllTrackedObjects()
	{
		foreach (var trackedObject in trackedObjects)
		{
			trackedObject.Value.SetActive(true);
		}
	}


    private bool isToShowCurrentObject = false;

    public void SetActiveOfTrackedObject()
    {
        if(isToShowCurrentObject)
        {
            EnableCurrentTrackedObject();
        } else
        {
            DisableCurrentTrackedObject();
        }

        isToShowCurrentObject = !isToShowCurrentObject;
    }

    public void EnableCurrentTrackedObject()
	{
		if (trackedObjects.ContainsKey(currentTrackedObjectName))
		{
			trackedObjects[currentTrackedObjectName].SetActive(true);
			StartVuforia();
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