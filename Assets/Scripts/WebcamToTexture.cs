using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class WebcamToTexture : MonoBehaviour, IVideoBackgroundEventHandler
{
	private Texture mVideoBackgroundTexture = null;
	private bool mVideoBgConfigChanged = false;

	// Use this for initialization
	void Start () {
		VuforiaARController.Instance.RegisterVideoBgEventHandler(this);
	}

	void OnDestroy()
	{
		// unregister for the OnVideoBackgroundConfigChanged event at the VuforiaBehaviour
		VuforiaARController.Instance.UnregisterVideoBgEventHandler(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (mVideoBgConfigChanged && VuforiaRenderer.Instance.IsVideoBackgroundInfoAvailable())
		{
			UpdateVideoTexture();
			mVideoBgConfigChanged = false;
		}
	}

	private void UpdateVideoTexture()
	{
		if (mVideoBackgroundTexture != VuforiaRenderer.Instance.VideoBackgroundTexture)
		{
			mVideoBackgroundTexture = VuforiaRenderer.Instance.VideoBackgroundTexture;
			GetComponent<Renderer>().material.mainTexture = mVideoBackgroundTexture;
		}
	}

	public void OnVideoBackgroundConfigChanged()
	{
		mVideoBgConfigChanged = true;
	}
}
