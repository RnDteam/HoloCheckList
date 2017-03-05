using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectOverlapCanvas : MonoBehaviour {

    public GameObject smallChecklist;

    private Vector3 linePoint;
    private Vector3 lineVec;
    private Vector3[] corners;

    void Start () {
        linePoint = Camera.main.transform.position;
        lineVec = smallChecklist.transform.position - linePoint;

        corners = new Vector3[4];
        smallChecklist.GetComponent<RectTransform>().GetWorldCorners(corners);
    }
	
	void Update () {
        smallChecklist.GetComponent<RectTransform>().GetWorldCorners(corners);

        bool isIntersects = false;

        foreach (var corner in corners)
        {
            isIntersects |= Physics.Raycast(linePoint, corner);
        }

        smallChecklist.SetActive(!isIntersects);
    }
}
