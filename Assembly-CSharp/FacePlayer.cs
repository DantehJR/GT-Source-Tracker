using UnityEngine;

public class FacePlayer : MonoBehaviour
{
	private void LateUpdate()
	{
		transform.rotation = Quaternion.LookRotation(transform.position - GorillaTagger.Instance.headCollider.transform.position) * Quaternion.AngleAxis(-90f, Vector3.up);
	}
}
