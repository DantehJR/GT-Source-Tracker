using UnityEngine;

[DefaultExecutionOrder(100)]
public class TransformFollow : MonoBehaviour
{
	public Transform transformToFollow;

	public Vector3 offset;

	public Vector3 prevPos;

	public bool rotationOnly;

	private bool forRigRecording;

	private TransformFollow parentFollow;

	private void Awake()
	{
		prevPos = transform.position;
		if (rotationOnly && transform.parent != null && transform.parent.GetComponent<TransformFollow>() != null)
		{
			forRigRecording = true;
		}
		if (forRigRecording)
		{
			parentFollow = transform.parent.GetComponent<TransformFollow>();
		}
	}

	private void LateUpdate()
	{
		prevPos = transform.position;
		if (rotationOnly)
		{
			if (forRigRecording)
			{
				transform.localRotation = Quaternion.Inverse(parentFollow.transformToFollow.rotation) * transformToFollow.rotation;
			}
			else
			{
				transform.rotation = transformToFollow.rotation;
			}
		}
		else
		{
			transformToFollow.GetPositionAndRotation(out var position, out var rotation);
			transform.SetPositionAndRotation(position + rotation * offset, rotation);
		}
	}
}
