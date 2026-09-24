using UnityEngine;

namespace GorillaTag;

public class DeactivateOnAwake : MonoBehaviour
{
	private void Awake()
	{
		gameObject.SetActive(value: false);
		Object.Destroy(this);
	}
}
