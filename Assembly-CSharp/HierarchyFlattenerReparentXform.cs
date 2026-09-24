using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class HierarchyFlattenerReparentXform : MonoBehaviour
{
	public Transform newParent;

	private bool _didIt;

	protected void Awake()
	{
		if (enabled)
		{
			_DoIt();
		}
	}

	protected void OnEnable()
	{
		_DoIt();
	}

	private void _DoIt()
	{
		if (!_didIt)
		{
			if (newParent != null)
			{
				transform.SetParent(newParent, worldPositionStays: true);
			}
			Object.Destroy(this);
		}
	}
}
