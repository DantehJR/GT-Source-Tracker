using UnityEngine;

public class GTDisableStaticOnAwake : MonoBehaviour
{
	private void Awake()
	{
		gameObject.isStatic = false;
		Object.Destroy(this);
	}
}
