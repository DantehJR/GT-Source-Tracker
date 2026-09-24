using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;

public class FriendshipCharm : HoldableObject
{
	[SerializeField]
	private InteractionPoint interactionPoint;

	[SerializeField]
	private Transform rightHandHoldAnchor;

	[SerializeField]
	private Transform leftHandHoldAnchor;

	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private Transform lineStart;

	[SerializeField]
	private Transform lineEnd;

	[SerializeField]
	private Transform releasePosition;

	[SerializeField]
	private float breakBraceletLength;

	[SerializeField]
	private LayerMask breakItemLayerMask;

	private Transform parent;

	private bool isBroken;

	private void Awake()
	{
		parent = transform.parent;
	}

	private void LateUpdate()
	{
		if (!isBroken && (lineStart.transform.position - lineEnd.transform.position).IsLongerThan(breakBraceletLength * GTPlayer.Instance.scale))
		{
			DestroyBracelet();
		}
	}

	public void OnEnable()
	{
		interactionPoint.enabled = true;
		meshRenderer.enabled = true;
		isBroken = false;
		UpdatePosition();
	}

	private void DestroyBracelet()
	{
		interactionPoint.enabled = false;
		isBroken = true;
		Debug.Log("LeaveGroup: bracelet destroyed");
		FriendshipGroupDetection.Instance.LeaveParty();
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 2f);
		transform.SetParent(flag ? leftHandHoldAnchor : rightHandHoldAnchor);
		transform.localPosition = Vector3.zero;
	}

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		bool forLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(null, forLeftHand);
		UpdatePosition();
		return base.OnRelease(zoneReleased, releasingHand);
	}

	private void UpdatePosition()
	{
		transform.SetParent(parent);
		transform.localPosition = releasePosition.localPosition;
		transform.localRotation = releasePosition.localRotation;
	}

	private void OnCollisionEnter(Collision other)
	{
		if (isBroken && (int)breakItemLayerMask == ((int)breakItemLayerMask | (1 << other.gameObject.layer)))
		{
			meshRenderer.enabled = false;
			UpdatePosition();
		}
	}

	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	public override void DropItemCleanup()
	{
	}
}
