using GorillaLocomotion;
using UnityEngine;

public class MatchGTPlayerRotation : MonoBehaviour
{
	public bool matchPosition;

	public bool matchRotation;

	private void LateUpdate()
	{
		if (matchPosition)
		{
			transform.position = GTPlayer.Instance.mainCamera.transform.position;
		}
		if (matchRotation)
		{
			transform.rotation = GTPlayer.Instance.transform.rotation;
		}
	}
}
