using UnityEngine;

public class PredicatableRandomRotation : MonoBehaviour
{
	[SerializeField]
	private Vector3 rot = Vector3.zero;

	[SerializeField]
	private Transform source;

	private void Start()
	{
		if (source == null)
		{
			source = transform;
		}
	}

	private void Update()
	{
		float num = (source.position.x * source.position.x + source.position.y * source.position.y + source.position.z * source.position.z) % 1f;
		transform.Rotate(rot * num);
	}
}
