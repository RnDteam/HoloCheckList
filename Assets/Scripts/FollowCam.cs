using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{

    private Vector3 offsetVector;
    private Quaternion defaultRotation;

    public float speed = 0.2f;

    void Start()
    {
        offsetVector = transform.position - Camera.main.transform.position;
        defaultRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + Camera.main.transform.TransformVector(offsetVector), speed);
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position) * defaultRotation;
    }
}