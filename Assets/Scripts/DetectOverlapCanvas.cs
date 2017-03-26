using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectOverlapCanvas : MonoBehaviour {

    public GameObject smallChecklist;

    private Vector3 linePoint;
    private Vector3[] corners;
	private Vector3 heading;
	private float distance;
	private Vector3 direction;

    void Start () {
        corners = new Vector3[4];
    }
	
	void Update () {
		linePoint = Camera.main.transform.position;
        smallChecklist.GetComponent<RectTransform>().GetWorldCorners(corners);

        bool isIntersects = false;

        foreach (var corner in corners)
        {
			heading = corner - linePoint;
			distance = heading.magnitude;
			direction = heading / distance;
			isIntersects |= Physics.Raycast(linePoint, direction);
        }

		heading = smallChecklist.transform.position - linePoint;
		distance = heading.magnitude;
		direction = heading / distance;

		isIntersects |= Physics.Raycast(linePoint, direction);

        //SetChildrenAppearance(smallChecklist, !isIntersects);
		smallChecklist.SetActive(!isIntersects);
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
