using BoingKit;
using UnityEngine;

public class SyncRigidBodyToMovement : MonoBehaviour
{
	[SerializeField]
	private Rigidbody targetRigidbody;

	private Transform targetParent;

	private void Awake()
	{
		targetParent = targetRigidbody.transform.parent;
		targetRigidbody.transform.parent = null;
		targetRigidbody.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		targetRigidbody.gameObject.SetActive(value: true);
		targetRigidbody.transform.position = transform.position;
		targetRigidbody.transform.rotation = transform.rotation;
	}

	private void OnDisable()
	{
		targetRigidbody.gameObject.SetActive(value: false);
	}

	private void FixedUpdate()
	{
		targetRigidbody.linearVelocity = (transform.position - targetRigidbody.position) / Time.fixedDeltaTime;
		targetRigidbody.angularVelocity = QuaternionUtil.ToAngularVector(Quaternion.Inverse(targetRigidbody.rotation) * transform.rotation) / Time.fixedDeltaTime;
	}
}
