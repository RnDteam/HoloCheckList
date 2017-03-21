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