using UnityEngine;

public class GorillaHandHistory : MonoBehaviour
{
	public Vector3 direction;

	private Vector3 lastPosition;

	private Vector3 lastLastPosition;

	private void Start()
	{
		direction = default;
		lastPosition = default;
	}

	private void FixedUpdate()
	{
		direction = lastPosition - transform.position;
		lastLastPosition = lastPosition;
		lastPosition = transform.position;
	}
}
