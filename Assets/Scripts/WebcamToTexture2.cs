using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebcamToTexture2 : MonoBehaviour {

	void Start() {
		WebCamTexture webcamTexture = new WebCamTexture();
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.mainTexture = webcamTexture;
		webcamTexture.Play();
	}
}
