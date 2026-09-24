using UnityEngine;

public class IgnoreLocalRotation : MonoBehaviour
{
	private void LateUpdate()
	{
		transform.rotation = Quaternion.identity;
	}
}
