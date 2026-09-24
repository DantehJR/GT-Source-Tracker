using UnityEngine;

public class FreeHoverboardInstance : MonoBehaviour
{
	public int ownerActorNumber;

	public int boardIndex;

	[SerializeField]
	private Vector3 sphereCastCenter;

	[SerializeField]
	private float sphereCastRadius;

	[SerializeField]
	private LayerMask hoverRaycastMask;

	[SerializeField]
	private float hoverHeight;

	[SerializeField]
	private float hoverRotationLerp;

	[SerializeField]
	private float avelocityDragWhileHovering;

	[SerializeField]
	private MeshRenderer boardMesh;

	private Material colorMaterial;

	private bool hasHoverPoint;

	private Vector3 hoverPoint;

	private Vector3 hoverNormal;

	public Rigidbody Rigidbody { get; private set; }

	public Color boardColor { get; private set; }

	private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody>();
		Material[] sharedMaterials = boardMesh.sharedMaterials;
		colorMaterial = new Material(sharedMaterials[1]);
		sharedMaterials[1] = colorMaterial;
		boardMesh.sharedMaterials = sharedMaterials;
	}

	public void SetColor(Color col)
	{
		colorMaterial.color = col;
		boardColor = col;
	}

	private void Update()
	{
		if (Physics.SphereCast(new Ray(transform.TransformPoint(sphereCastCenter), transform.TransformVector(Vector3.down)), sphereCastRadius, out var hitInfo, 1f, hoverRaycastMask.value))
		{
			hasHoverPoint = true;
			hoverPoint = hitInfo.point;
			hoverNormal = hitInfo.normal;
		}
		else
		{
			hasHoverPoint = false;
		}
	}

	private void FixedUpdate()
	{
		if (hasHoverPoint)
		{
			float num = Vector3.Dot(transform.TransformPoint(sphereCastCenter) - hoverPoint, hoverNormal);
			if (num < hoverHeight)
			{
				transform.position += hoverNormal * (hoverHeight - num);
				Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, hoverNormal);
				Vector3 vector = Quaternion.Inverse(transform.rotation) * Rigidbody.angularVelocity;
				vector.x *= avelocityDragWhileHovering;
				vector.z *= avelocityDragWhileHovering;
				Rigidbody.angularVelocity = transform.rotation * vector;
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hoverNormal), hoverNormal), hoverRotationLerp);
			}
		}
	}
}
