using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GorillaIKHandTarget : MonoBehaviour
{
	public GameObject handToStickTo;

	public bool isLeftHand;

	public float hapticStrength;

	private Rigidbody thisRigidbody;

	private XRController controllerReference;

	private void Start()
	{
		thisRigidbody = gameObject.GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		thisRigidbody.MovePosition(handToStickTo.transform.position);
		transform.rotation = handToStickTo.transform.rotation;
	}

	private void OnCollisionEnter(Collision collision)
	{
	}
}
