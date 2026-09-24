using UnityEngine;

public class LookAtTransform : MonoBehaviour
{
	[SerializeField]
	private Transform lookAt;

	private void Update()
	{
		transform.rotation = Quaternion.LookRotation(lookAt.position - transform.position);
	}
}
