using UnityEngine;

namespace BoingKit;

public class BoingBoneCollider : MonoBehaviour
{
	public enum Type
	{
		Sphere,
		Capsule,
		Box
	}

	public Type Shape;

	public float Radius = 0.1f;

	public float Height = 0.25f;

	public Vector3 Dimensions = new Vector3(0.1f, 0.1f, 0.1f);

	public Bounds Bounds
	{
		get
		{
			switch (Shape)
			{
			case Type.Sphere:
			{
				float num2 = VectorUtil.MinComponent(transform.localScale);
				return new Bounds(transform.position, 2f * num2 * Radius * Vector3.one);
			}
			case Type.Capsule:
			{
				float num = VectorUtil.MinComponent(transform.localScale);
				return new Bounds(transform.position, 2f * num * Radius * Vector3.one + Height * VectorUtil.ComponentWiseAbs(transform.rotation * Vector3.up));
			}
			case Type.Box:
				return new Bounds(transform.position, VectorUtil.ComponentWiseMult(transform.localScale, VectorUtil.ComponentWiseAbs(transform.rotation * Dimensions)));
			default:
				return default;
			}
		}
	}

	public bool Collide(Vector3 boneCenter, float boneRadius, out Vector3 push)
	{
		switch (Shape)
		{
		case Type.Sphere:
		{
			float num2 = VectorUtil.MinComponent(transform.localScale);
			return Collision.SphereSphere(boneCenter, boneRadius, transform.position, num2 * Radius, out push);
		}
		case Type.Capsule:
		{
			float num = VectorUtil.MinComponent(transform.localScale);
			Vector3 headB = transform.TransformPoint(0.5f * Height * Vector3.up);
			Vector3 tailB = transform.TransformPoint(0.5f * Height * Vector3.down);
			return Collision.SphereCapsule(boneCenter, boneRadius, headB, tailB, num * Radius, out push);
		}
		case Type.Box:
		{
			Vector3 centerOffsetA = transform.InverseTransformPoint(boneCenter);
			Vector3 halfExtentB = 0.5f * VectorUtil.ComponentWiseMult(transform.localScale, Dimensions);
			if (!Collision.SphereBox(centerOffsetA, boneRadius, halfExtentB, out push))
			{
				return false;
			}
			push = transform.TransformVector(push);
			return true;
		}
		default:
			push = Vector3.zero;
			return false;
		}
	}

	public void OnValidate()
	{
		Radius = Mathf.Max(0f, Radius);
		Dimensions.x = Mathf.Max(0f, Dimensions.x);
		Dimensions.y = Mathf.Max(0f, Dimensions.y);
		Dimensions.z = Mathf.Max(0f, Dimensions.z);
	}

	public void OnDrawGizmos()
	{
		DrawGizmos();
	}

	public void DrawGizmos()
	{
		switch (Shape)
		{
		case Type.Sphere:
		{
			float radius = VectorUtil.MinComponent(transform.localScale) * Radius;
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			if (Shape == Type.Sphere)
			{
				Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
				Gizmos.DrawSphere(Vector3.zero, radius);
			}
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(Vector3.zero, radius);
			Gizmos.matrix = Matrix4x4.identity;
			break;
		}
		case Type.Capsule:
		{
			float num = VectorUtil.MinComponent(transform.localScale);
			float num2 = num * Radius;
			float num3 = 0.5f * num * Height;
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			if (Shape == Type.Capsule)
			{
				Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
				Gizmos.DrawSphere(num3 * Vector3.up, num2);
				Gizmos.DrawSphere(num3 * Vector3.down, num2);
			}
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(num3 * Vector3.up, num2);
			Gizmos.DrawWireSphere(num3 * Vector3.down, num2);
			for (int i = 0; i < 4; i++)
			{
				float f = (float)i * MathUtil.HalfPi;
				Vector3 vector = new Vector3(num2 * Mathf.Cos(f), 0f, num2 * Mathf.Sin(f));
				Gizmos.DrawLine(vector + num3 * Vector3.up, vector + num3 * Vector3.down);
			}
			Gizmos.matrix = Matrix4x4.identity;
			break;
		}
		case Type.Box:
		{
			Vector3 size = VectorUtil.ComponentWiseMult(transform.localScale, Dimensions);
			Gizmos.matrix = transform.localToWorldMatrix;
			if (Shape == Type.Box)
			{
				Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
				Gizmos.DrawCube(Vector3.zero, size);
			}
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(Vector3.zero, size);
			Gizmos.matrix = Matrix4x4.identity;
			break;
		}
		}
	}
}
