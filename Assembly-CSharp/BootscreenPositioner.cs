using UnityEngine;

public class BootscreenPositioner : MonoBehaviour
{
	[SerializeField]
	private Transform _pov;

	[SerializeField]
	private float _distanceThreshold;

	[SerializeField]
	private float _rotationThreshold;

	private void Awake()
	{
		transform.position = _pov.position;
		transform.rotation = Quaternion.Euler(0f, _pov.rotation.eulerAngles.y, 0f);
	}

	private void LateUpdate()
	{
		if (Vector3.Distance(transform.position, _pov.position) > _distanceThreshold)
		{
			transform.position = _pov.position;
		}
		if (Mathf.Abs(_pov.rotation.eulerAngles.y - transform.rotation.eulerAngles.y) > _rotationThreshold)
		{
			transform.rotation = Quaternion.Euler(0f, _pov.rotation.eulerAngles.y, 0f);
		}
	}
}
