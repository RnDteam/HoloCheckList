using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedObjectsContainer : MonoBehaviour {

	public static TrackedObjectsContainer Instance;

	private Dictionary<string, GameObject> trackedObjects = new Dictionary<string, GameObject>();

	// Use this for initialization
	void Awake () {
		Instance = this;
		foreach(Transform trackedObject in transform)
		{
			trackedObjects.Add(trackedObject.name, trackedObject.gameObject);
		}
		DisableAllTrackedObjects();
	}

	public void EnableTrackedObject(string objectName)
	{
		if (trackedObjects.ContainsKey(objectName))
		{
			trackedObjects[objectName].SetActive(true);
		}
	}

	public void DisableTrackedObject(string objectName)
	{
		if (trackedObjects.ContainsKey(objectName))
		{
			trackedObjects[objectName].SetActive(false);
		}
	}

	public void DisableAllTrackedObjects()
	{
		foreach (var trackedObject in trackedObjects)
		{
			trackedObject.Value.SetActive(false);
		}
	}
	
}