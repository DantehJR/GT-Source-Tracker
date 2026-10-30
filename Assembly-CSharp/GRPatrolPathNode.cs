using UnityEngine;

[AddComponentMenu("GorillaTag/Ghost Reactor/GR Patrol Path Node")]
public class GRPatrolPathNode : MonoBehaviour
{
	public void OnDrawGizmosSelected()
	{
		if (!(transform.parent == null))
		{
			GRPatrolPath component = transform.parent.GetComponent<GRPatrolPath>();
			if (!(component == null))
			{
				component.OnDrawGizmosSelected();
			}
		}
	}
}
