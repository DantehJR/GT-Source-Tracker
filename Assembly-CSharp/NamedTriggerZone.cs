using UnityEngine;

public class NamedTriggerZone : MonoBehaviour
{
	public string TriggerName = "Trigger";

	private void Reset()
	{
		ConfigureCollider();
	}

	private void ConfigureCollider()
	{
		Collider collider = GetComponent<Collider>();
		if (!collider)
		{
			collider = gameObject.AddComponent<BoxCollider>();
		}
		collider.isTrigger = true;
		gameObject.layer = LayerMask.NameToLayer("Gorilla Trigger");
	}
}
