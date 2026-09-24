using UnityEngine;

public class LockRotation : MonoBehaviour
{
	private Quaternion lockedRot;

	private void Start()
	{
		lockedRot = transform.rotation;
	}

	private void LateUpdate()
	{
		transform.rotation = lockedRot;
	}
}
