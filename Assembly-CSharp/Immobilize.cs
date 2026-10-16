using UnityEngine;

public class Immobilize : MonoBehaviour
{
	private Vector3 lockedPosition;

	private Quaternion lockedRotation;

	private void Start()
	{
		lockedPosition = transform.position;
		lockedRotation = transform.rotation;
	}

	private void LateUpdate()
	{
		transform.position = lockedPosition;
		transform.rotation = lockedRotation;
	}
}
