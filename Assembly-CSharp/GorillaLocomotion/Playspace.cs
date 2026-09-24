using Unity.XR.CoreUtils;
using UnityEngine;

namespace GorillaLocomotion;

public sealed class Playspace : MonoBehaviour
{
	[SerializeField]
	private GameObject _localGorillaHead;

	[SerializeField]
	private float _sphereRadius;

	private float _sqrSphereRadius;

	[SerializeField]
	private float _defaultChaseSpeed;

	[SerializeField]
	private float _snapToThreshold;

	private float _sqrSnapToThreshold;

	[SerializeField]
	private GTPlayer m_gtPlayer;

	[SerializeField]
	private XROrigin m_xrOrigin;

	private Transform m_xrBody;

	private void Awake()
	{
		_sqrSphereRadius = _sphereRadius * _sphereRadius;
		_sqrSnapToThreshold = _snapToThreshold * _snapToThreshold;
	}

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 vector = _localGorillaHead.transform.position - transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		if (GTPlayer.Instance.enableHoverMode || GTPlayer.Instance.isClimbing || vector.sqrMagnitude > _sqrSnapToThreshold)
		{
			transform.position = _localGorillaHead.transform.position;
			return;
		}
		Vector3 normalized = vector.normalized;
		vector = GetChaseSpeed() * Time.deltaTime * normalized;
		transform.position = ((vector.sqrMagnitude > sqrMagnitude) ? _localGorillaHead.transform.position : (transform.position + vector));
		if ((_localGorillaHead.transform.position - transform.position).sqrMagnitude > _sqrSphereRadius)
		{
			_localGorillaHead.transform.position = transform.position + _sphereRadius * normalized;
		}
	}

	private float GetChaseSpeed()
	{
		return _defaultChaseSpeed;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, _sphereRadius);
	}
}
