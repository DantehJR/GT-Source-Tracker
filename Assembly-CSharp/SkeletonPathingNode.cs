using UnityEngine;

[AddComponentMenu("GorillaTag/Environment/Skeleton Pathing Node")]
public class SkeletonPathingNode : MonoBehaviour
{
	public bool ejectionPoint;

	public SkeletonPathingNode[] connectedNodes;

	public float distanceToExitNode;

	private void Awake()
	{
		gameObject.SetActive(value: false);
	}
}
