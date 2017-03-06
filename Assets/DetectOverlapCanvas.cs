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
        Debug.Log(corners[0]);
        Debug.Log(corners[1]);
        Debug.Log(corners[2]);
        Debug.Log(corners[3]);
    }
	
	void Update () {
        smallChecklist.GetComponent<RectTransform>().GetWorldCorners(corners);

        bool isIntersects = false;

        foreach (var corner in corners)
        {
            isIntersects |= Physics.Raycast(linePoint, corner);
        }

        SetChildrenAppearance(smallChecklist, !isIntersects);
    }

    void SetChildrenAppearance(GameObject go, bool bIsDisplayed)
    {
        Transform[] ts = go.GetComponentsInChildren<RectTransform>();
        

        if (ts == null)
            return;

        foreach (Transform t in go.transform)
        {
            t.gameObject.SetActive(bIsDisplayed);
        }
    }
}
