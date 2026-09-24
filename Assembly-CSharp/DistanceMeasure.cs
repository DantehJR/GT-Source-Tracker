using UnityEngine;

public class DistanceMeasure : MonoBehaviour
{
	public Transform from;

	public Transform to;

	private void Awake()
	{
		if (from == null)
		{
			from = transform;
		}
		if (to == null)
		{
			to = transform;
		}
	}
}
