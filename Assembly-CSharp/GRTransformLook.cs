using UnityEngine;

[AddComponentMenu("GorillaTag/Ghost Reactor/GR Transform Look")]
public class GRTransformLook : MonoBehaviour
{
	public bool followPlayer;

	public Transform lookTarget;

	public Vector3 offsetRotation;

	private void Awake()
	{
		if (followPlayer)
		{
			lookTarget = Camera.main.transform;
		}
	}

	private void LateUpdate()
	{
		transform.LookAt(lookTarget);
		transform.rotation *= Quaternion.Euler(offsetRotation);
	}
}
