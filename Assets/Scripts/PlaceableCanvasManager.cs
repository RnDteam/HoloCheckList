using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableCanvasManager : MonoBehaviour
{

    public Vector3 offsetVector;
    private Quaternion defaultRotation;
    public float speed;
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

        transform.position = Vector3.Lerp(transform.position,
                                          Camera.main.transform.position + Camera.main.transform.TransformVector(offsetVector),
                                          speed);
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