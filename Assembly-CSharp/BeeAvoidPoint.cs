using UnityEngine;

public class BeeAvoidPoint : MonoBehaviour
{
	private void Start()
	{
		BeeSwarmManager.RegisterAvoidPoint(gameObject);
		FlockingManager.RegisterAvoidPoint(gameObject);
	}

	private void OnDestroy()
	{
		BeeSwarmManager.UnregisterAvoidPoint(gameObject);
		FlockingManager.UnregisterAvoidPoint(gameObject);
	}
}
