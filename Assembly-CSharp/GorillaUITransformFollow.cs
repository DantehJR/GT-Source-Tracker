using UnityEngine;

public class GorillaUITransformFollow : MonoBehaviour
{
	public Transform transformToFollow;

	public Vector3 offset;

	public bool doesMove;

	private void Start()
	{
	}

	private void LateUpdate()
	{
		if (doesMove)
		{
			transform.rotation = transformToFollow.rotation;
			transform.position = transformToFollow.position + transformToFollow.rotation * offset;
		}
	}
}
