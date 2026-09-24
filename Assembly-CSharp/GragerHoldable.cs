using GorillaExtensions;
using Unity.Cinemachine;
using UnityEngine;

public class GragerHoldable : MonoBehaviour
{
	[SerializeField]
	private Vector3 LocalCenterOfMass;

	[SerializeField]
	private Vector3 LocalRotationAxis;

	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	[SerializeField]
	private float drag;

	[SerializeField]
	private float gravity;

	[SerializeField]
	private float localFriction;

	[SerializeField]
	private float distancePerClack;

	[SerializeField]
	private AudioSource clackAudio;

	[SerializeField]
	private AudioClip[] allClacks;

	private float centerOfMassRadius;

	private Vector3 velocity;

	private Vector3 lastWorldPosition;

	private Vector3 lastClackParentLocalPosition;

	private Quaternion RotationCorrection;

	private void Start()
	{
		LocalRotationAxis = LocalRotationAxis.normalized;
		lastWorldPosition = transform.TransformPoint(LocalCenterOfMass);
		lastClackParentLocalPosition = transform.parent.InverseTransformPoint(lastWorldPosition);
		centerOfMassRadius = LocalCenterOfMass.magnitude;
		RotationCorrection = Quaternion.Euler(RotationCorrectionEuler);
	}

	private void Update()
	{
		Vector3 target = transform.TransformPoint(LocalCenterOfMass);
		Vector3 vector = lastWorldPosition + velocity * Time.deltaTime * drag;
		Vector3 vector2 = transform.parent.TransformDirection(LocalRotationAxis);
		Vector3 current = transform.position + (vector - transform.position).ProjectOntoPlane(vector2).normalized * centerOfMassRadius;
		current = Vector3.MoveTowards(current, target, localFriction * Time.deltaTime);
		velocity = (current - lastWorldPosition) / Time.deltaTime;
		velocity += Vector3.down * gravity * Time.deltaTime;
		lastWorldPosition = current;
		transform.rotation = Quaternion.LookRotation(current - transform.position, vector2) * RotationCorrection;
		Vector3 vector3 = transform.parent.InverseTransformPoint(transform.TransformPoint(LocalCenterOfMass));
		if ((vector3 - lastClackParentLocalPosition).IsLongerThan(distancePerClack))
		{
			clackAudio.GTPlayOneShot(allClacks[Random.Range(0, allClacks.Length)]);
			lastClackParentLocalPosition = vector3;
		}
	}
}
