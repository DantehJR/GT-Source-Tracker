using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics;

public class FartBagThrowable : MonoBehaviour, IProjectile
{
	[SerializeField]
	private GameObject deflationEffect;

	[SerializeField]
	private float destroyWhenDeflateDelay = 3f;

	[SerializeField]
	private float forceDestroyAfterSec = 10f;

	[SerializeField]
	private float placementOffset = 0.2f;

	[SerializeField]
	private UpdateBlendShapeCosmetic updateBlendShapeCosmetic;

	[SerializeField]
	private LayerMask floorLayerMask;

	[SerializeField]
	private LayerMask handLayerMask;

	[SerializeField]
	private Rigidbody rigidbody;

	private bool placedOnFloor;

	private float placedOnFloorTime;

	private float timeCreated;

	private bool deflated;

	private Vector3 handContactPoint;

	private Vector3 handNormalVector;

	private CallLimiter callLimiter = new CallLimiter(10, 2f);

	private RubberDuckEvents _events;

	public TransferrableObject ParentTransferable { get; set; }

	public event Action<IProjectile> OnDeflated;

	private void OnEnable()
	{
		placedOnFloor = false;
		deflated = false;
		handContactPoint = Vector3.negativeInfinity;
		handNormalVector = Vector3.zero;
		timeCreated = float.PositiveInfinity;
		placedOnFloorTime = float.PositiveInfinity;
		if ((bool)updateBlendShapeCosmetic)
		{
			updateBlendShapeCosmetic.ResetBlend();
		}
	}

	private void Update()
	{
		if (Time.time - timeCreated > forceDestroyAfterSec)
		{
			DeflateLocal();
		}
	}

	public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
	{
		transform.position = startPosition;
		transform.rotation = startRotation;
		transform.localScale = Vector3.one * ownerRig.scaleFactor;
		rigidbody.linearVelocity = velocity;
		timeCreated = Time.time;
		InitialPhotonEvent();
	}

	private void InitialPhotonEvent()
	{
		_events = gameObject.GetOrAddComponent<RubberDuckEvents>();
		if ((bool)ParentTransferable)
		{
			NetPlayer netPlayer;
			if (ParentTransferable.myOnlineRig != null)
			{
				netPlayer = ParentTransferable.myOnlineRig.creator;
			}
			else
			{
				netPlayer = ((ParentTransferable.myRig != null) ? (ParentTransferable.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
			}
			if (_events != null && netPlayer != null)
			{
				_events.Init(netPlayer);
			}
		}
		if (_events != null)
		{
			_events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(DeflateEvent);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((handLayerMask.value & (1 << other.gameObject.layer)) != 0 && placedOnFloor)
		{
			handContactPoint = other.ClosestPoint(transform.position);
			handNormalVector = (handContactPoint - transform.position).normalized;
			if (Time.time - placedOnFloorTime > 0.3f)
			{
				Deflate();
			}
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if ((floorLayerMask.value & (1 << other.gameObject.layer)) != 0)
		{
			placedOnFloor = true;
			placedOnFloorTime = Time.time;
			Vector3 normal = other.contacts[0].normal;
			transform.position = other.contacts[0].point + normal * placementOffset;
			Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, normal).normalized, normal);
			transform.rotation = rotation;
		}
	}

	private void Deflate()
	{
		if (PhotonNetwork.InRoom && _events != null && _events.Activate != null)
		{
			_events.Activate.RaiseOthers(handContactPoint, handNormalVector);
		}
		DeflateLocal();
	}

	private void DeflateEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender == target && args.Length == 2)
		{
			MonkeAgent.IncrementRPCCall(info, "DeflateEvent");
			if (callLimiter.CheckCallTime(Time.time) && args[0] is Vector3 v && args[1] is Vector3 v2 && v2.IsValid(10000f) && v.IsValid(10000f) && ParentTransferable.targetRig.IsPositionInRange(v, 4f))
			{
				handNormalVector = v2;
				handContactPoint = v;
				DeflateLocal();
			}
		}
	}

	private void DeflateLocal()
	{
		if (!deflated)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(deflationEffect, handContactPoint);
			gameObject.transform.up = handNormalVector;
			gameObject.transform.position = transform.position;
			SoundBankPlayer componentInChildren = gameObject.GetComponentInChildren<SoundBankPlayer>();
			if ((bool)componentInChildren.soundBank)
			{
				componentInChildren.Play();
			}
			placedOnFloor = false;
			timeCreated = float.PositiveInfinity;
			if ((bool)updateBlendShapeCosmetic)
			{
				updateBlendShapeCosmetic.FullyBlend();
			}
			deflated = true;
			Invoke("DisableObject", destroyWhenDeflateDelay);
		}
	}

	private void DisableObject()
	{
		OnDeflated?.Invoke(this);
		deflated = false;
	}

	private void OnDestroy()
	{
		if (_events != null)
		{
			_events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(DeflateEvent);
			_events.Dispose();
			_events = null;
		}
	}
}
