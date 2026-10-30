using UnityEngine;

[AddComponentMenu("GorillaTag/Cosmetics/Ignore Local Rotation")]
public class IgnoreLocalRotation : MonoBehaviour
{
	private void LateUpdate()
	{
		transform.rotation = Quaternion.identity;
	}
}
