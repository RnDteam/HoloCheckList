using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public Vector3 offsetVector;
    private Quaternion defaultRotation;
    public float speed;
    public bool isPlaced;

    void Start()
    {
        //offsetVector = transform.position - Camera.main.transform.position;
        defaultRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (isPlaced)
            return;

        transform.position = Vector3.Lerp(transform.position,
                                          Camera.main.transform.position + Camera.main.transform.TransformVector(offsetVector),
                                          speed);
		transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position) * defaultRotation;
    }

    public void PlaceCanvas()
    {
        isPlaced = true;
		gameObject.GetComponentInChildren<bigChecklistManager>().Placed();
        TaskManager.SomethingWasPlaced();
    }

    public void MoveCanvas()
    {
        isPlaced = false;
		gameObject.GetComponentInChildren<bigChecklistManager>().Moved();
    }
}