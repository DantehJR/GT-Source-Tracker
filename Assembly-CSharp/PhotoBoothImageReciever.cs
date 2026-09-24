using System;
using UnityEngine;

public class PhotoBoothImageReciever : MonoBehaviour
{
	[SerializeField]
	private PhotoBoothCamera photoBoothCamera;

	[SerializeField]
	private int index = -1;

	private void OnEnable()
	{
		PhotoBoothCamera photoBoothCamera = this.photoBoothCamera;
		photoBoothCamera.OnCapture = (Action<Texture, int>)Delegate.Combine(photoBoothCamera.OnCapture, new Action<Texture, int>(photoBoothCamera_OnCapture));
	}

	private void photoBoothCamera_OnCapture(Texture texture, int i)
	{
		if (index < 0 || index == i)
		{
			GetComponent<Renderer>().material.mainTexture = texture;
		}
	}

	private void OnDisable()
	{
		PhotoBoothCamera photoBoothCamera = this.photoBoothCamera;
		photoBoothCamera.OnCapture = (Action<Texture, int>)Delegate.Remove(photoBoothCamera.OnCapture, new Action<Texture, int>(photoBoothCamera_OnCapture));
	}
}
