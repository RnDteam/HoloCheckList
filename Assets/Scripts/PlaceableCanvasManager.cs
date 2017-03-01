using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableCanvasManager : MonoBehaviour
{

    private Vector3 offsetVector;
    private Quaternion defaultRotation;

    public bool isPlaced;

    void Start()
    {
        offsetVector = transform.position - Camera.main.transform.position;
        defaultRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        if (isPlaced)
            return;
        transform.position = Camera.main.transform.position + Camera.main.transform.TransformVector(offsetVector);
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position) * defaultRotation;
    }

    public void PlaceCanvas()
    {
        isPlaced = true;
    }

    public void MoveCanvas()
    {
        isPlaced = false;
    }
}