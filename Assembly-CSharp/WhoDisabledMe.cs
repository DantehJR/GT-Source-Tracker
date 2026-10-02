using UnityEngine;

public class WhoDisabledMe : MonoBehaviour
{
	private bool destroying;

	private void OnDisable()
	{
		if (!gameObject.activeSelf)
		{
			Debug.Log("[WhoDisabledMe] " + name + " disabled directly", this);
			return;
		}
		Transform parent = transform.parent;
		while (parent != null && parent.gameObject.activeSelf)
		{
			parent = parent.parent;
		}
		Debug.Log("[WhoDisabledMe] " + name + " disabled via ancestor '" + (parent ? parent.name : "?") + "'", this);
	}
}
