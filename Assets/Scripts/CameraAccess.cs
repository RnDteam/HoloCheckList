using UnityEngine;
using System.Collections;
using Vuforia;
public class CameraAccess : MonoBehaviour
{

	public Renderer renderer;
	Texture2D newTex;

	private bool mAccessCameraImage = true;
	      
	// The desired camera image pixel format
	private Image.PIXEL_FORMAT mPixelFormat = Image.PIXEL_FORMAT.RGBA8888;// or RGBA8888, RGB888, RGB565, YUV
	// Boolean flag telling whether the pixel format has been registered
	private bool mFormatRegistered = false;

	void Start ()
	{
		        // Register Vuforia life-cycle callbacks:
		VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
		VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);
		VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
		newTex = new Texture2D(100,100);
		renderer.material.mainTexture = newTex;
	}
	/// <summary>
	/// Called when Vuforia is started
	/// </summary>
	private void OnVuforiaStarted()
	{
		// Try register camera image format
		if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
		{
			Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());
			mFormatRegistered = true;
		}
		else
		{
			Debug.LogError("Failed to register pixel format " + mPixelFormat.ToString() +
				"\n the format may be unsupported by your device;" +
				"\n consider using a different pixel format.");
			mFormatRegistered = false;
		}
	}
	/// <summary>
	/// Called when app is paused / resumed
	/// </summary>
	private void OnPause(bool paused)
	{
		if (paused)
		{
			Debug.Log("App was paused");
			UnregisterFormat();
		}
		else
		{
			Debug.Log("App was resumed");
			RegisterFormat();
		}
	}
	/// <summary>
	/// Called each time the Vuforia state is updated
	/// </summary>
	private void OnTrackablesUpdated()
	{
		if (mFormatRegistered)
		{
			if (mAccessCameraImage)
			{
				Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);
				if (image != null)
				{
					string imageInfo = mPixelFormat + " image: \n";
					imageInfo += " size: " + image.Width + " x " + image.Height + "\n";
					imageInfo += " bufferSize: " + image.BufferWidth + " x " + image.BufferHeight + "\n";
					imageInfo += " stride: " + image.Stride;
					Debug.Log(imageInfo);
					byte[] pixels = image.Pixels;
					if (pixels != null && pixels.Length > 0)
					{
						Debug.Log("Image pixels: " + pixels[0] + "," + pixels[1] + "," + pixels[2] + ",...");
						newTex.LoadImage(pixels);
					}
				}
			}
		}
	}
	/// <summary>
	/// Unregister the camera pixel format (e.g. call this when app is paused)
	/// </summary>
	private void UnregisterFormat()
	{
		Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
		CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
		mFormatRegistered = false;
	}
	/// <summary>
	/// Register the camera pixel format
	/// </summary>
	private void RegisterFormat()
	{
		if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
		{
			Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
			mFormatRegistered = true;
		}
		else
		{
			Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
			mFormatRegistered = false;
		}
	}
}