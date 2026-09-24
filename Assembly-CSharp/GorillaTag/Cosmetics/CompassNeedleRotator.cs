using UnityEngine;

namespace GorillaTag.Cosmetics;

public class CompassNeedleRotator : MonoBehaviour
{
	private const float smoothTime = 0.005f;

	private float currentVelocity;

	protected void OnEnable()
	{
		currentVelocity = 0f;
		transform.localRotation = Quaternion.identity;
	}

	protected void LateUpdate()
	{
		Transform transform = base.transform;
		Vector3 forward = transform.forward;
		forward.y = 0f;
		forward.Normalize();
		transform.Rotate(angle: Mathf.SmoothDamp(Vector3.SignedAngle(forward, Vector3.forward, Vector3.up), 0f, ref currentVelocity, 0.005f), axis: transform.up, relativeTo: Space.World);
	}
}
