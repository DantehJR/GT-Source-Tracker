using UnityEngine;

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
