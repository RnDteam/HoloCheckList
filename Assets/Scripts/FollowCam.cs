using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{

    private Vector3 offsetVector;
    private Quaternion defaultRotation;
    private Vector3 prevPos;
    public float speed = 0.2f;

	public bool stick;

    void Start()
    {
        offsetVector = transform.position - Camera.main.transform.position;
        defaultRotation = transform.rotation;
    }

    void LateUpdate()
    {
		if (stick)
		{
			transform.position = Camera.main.transform.position + Camera.main.transform.TransformVector(offsetVector);
			transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position) * defaultRotation;
		}
		else
		{
			prevPos = transform.position;

			transform.position = Camera.main.transform.position + Camera.main.transform.TransformVector(new Vector3(0, -1, 810));
			transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position) * defaultRotation;

			transform.position = Vector3.Lerp(prevPos, Camera.main.transform.position + Camera.main.transform.TransformVector(offsetVector), speed);
			//transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position) * defaultRotation;
		}
    }
}