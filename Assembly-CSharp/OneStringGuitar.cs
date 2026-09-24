using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

public class OneStringGuitar : TransferrableObject
{
	private enum GuitarStates
	{
		Club = 1,
		HeldReverseGrip = 2,
		Playing = 4
	}

	public Vector3 chestOffsetLeft;

	public Vector3 chestOffsetRight;

	public Quaternion holdingOffsetRotationLeft;

	public Quaternion holdingOffsetRotationRight;

	public Quaternion chestRotationOffset;

	[NonSerialized]
	public Collider currentChestCollider;

	[NonSerialized]
	public Collider chestColliderLeft;

	[NonSerialized]
	public Collider chestColliderRight;

	public float lerpValue = 0.25f;

	public AudioSource audioSource;

	private Transform parentHand;

	private Transform parentHandLeft;

	private Transform parentHandRight;

	public float unsnapDistance;

	public float snapDistance;

	public Vector3 startPositionLeft;

	public Quaternion startQuatLeft;

	public Vector3 reverseGripPositionLeft;

	public Quaternion reverseGripQuatLeft;

	public Vector3 startPositionRight;

	public Quaternion startQuatRight;

	public Vector3 reverseGripPositionRight;

	public Quaternion reverseGripQuatRight;

	public float angleLerpSnap = 1f;

	public float vectorLerpSnap = 0.01f;

	private bool angleSnapped;

	private bool positionSnapped;

	public Transform chestTouch;

	private int collidersHitCount;

	private Collider[] collidersHit = new Collider[20];

	private RaycastHit[] raycastHits = new RaycastHit[20];

	private List<RaycastHit> raycastHitList = new List<RaycastHit>();

	private RaycastHit nullHit;

	public Collider[] collidersToBeIn;

	public LayerMask interactableMask;

	public int currentFretIndex;

	public int lastFretIndex;

	public Collider[] frets;

	private List<Collider> fretsList = new List<Collider>();

	public AudioClip[] audioClips;

	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	private GorillaTriggerColliderHandIndicator fretHandIndicator;

	private GorillaTriggerColliderHandIndicator strumHandIndicator;

	private float sphereRadius;

	private bool anyHit;

	private bool handIn;

	private Vector3 spherecastSweep;

	public Collider strumCollider;

	public float maxVolume = 1f;

	public float minVolume = 0.05f;

	public float maxVelocity = 2f;

	private List<Collider> strumList = new List<Collider>();

	private int selfInstrumentIndex = -1;

	private GuitarStates lastState;

	private Vector3 startingLeftChestOffset;

	private Vector3 startingRightChestOffset;

	private float startingUnsnapDistance;

	public override Matrix4x4 GetDefaultTransformationMatrix()
	{
		return Matrix4x4.identity;
	}

	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		chestColliderLeft = _GetChestColliderByPath(rig, "rig/body_pivot/Old Cosmetics Body/OneStringGuitarStick/Center/BaseTransformLeft");
		chestColliderRight = _GetChestColliderByPath(rig, "rig/body_pivot/Old Cosmetics Body/OneStringGuitarStick/Center/BaseTransformRight");
		currentChestCollider = chestColliderLeft;
		if (!GTHardCodedBones.TryGetBoneXforms(rig, out var outBoneXforms, out var outErrorMsg))
		{
			Debug.LogError("OneStringGuitar: Error getting bone Transforms: " + outErrorMsg, this);
			return;
		}
		parentHandLeft = outBoneXforms[9];
		parentHandRight = outBoneXforms[27];
		parentHand = parentHandRight;
		leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		sphereRadius = leftHandIndicator.GetComponent<SphereCollider>().radius;
		itemState = ItemStates.State0;
		nullHit = default;
		strumList.Add(strumCollider);
		lastState = GuitarStates.Club;
		startingLeftChestOffset = chestOffsetLeft;
		startingRightChestOffset = chestOffsetRight;
		startingUnsnapDistance = unsnapDistance;
		selfInstrumentIndex = rig.AssignInstrumentToInstrumentSelfOnly(this);
		for (int i = 0; i < frets.Length; i++)
		{
			fretsList.Add(frets[i]);
		}
	}

	private Collider _GetChestColliderByPath(VRRig vrRig, string chestColliderLeftPath)
	{
		if (!vrRig.transform.TryFindByExactPath(chestColliderLeftPath, out var result))
		{
			Debug.LogError("DEACTIVATING! do you move this without updating the script? could not find this transform: \"" + chestColliderLeftPath + "\"");
			gameObject.SetActive(value: false);
		}
		Collider component = result.GetComponent<Collider>();
		if (!component)
		{
			Debug.LogError("DEACTIVATING! found transform but couldn't find collider at path: \"" + chestColliderLeftPath + "\"");
			gameObject.SetActive(value: false);
		}
		return component;
	}

	internal override void OnEnable()
	{
		base.OnEnable();
		if (currentState == PositionState.InLeftHand)
		{
			fretHandIndicator = leftHandIndicator;
			strumHandIndicator = rightHandIndicator;
		}
		else
		{
			fretHandIndicator = rightHandIndicator;
			strumHandIndicator = leftHandIndicator;
		}
		if (IsLocalObject())
		{
			parentHand = GTPlayer.Instance.GetHandFollower(currentState == PositionState.InLeftHand);
		}
		initOffset = Vector3.zero;
		initRotation = Quaternion.identity;
	}

	internal override void OnDisable()
	{
		base.OnDisable();
		angleSnapped = false;
		positionSnapped = false;
		lastState = GuitarStates.Club;
		itemState = ItemStates.State0;
	}

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!CanDeactivate())
		{
			return false;
		}
		if (InHand())
		{
			return false;
		}
		itemState = ItemStates.State0;
		return true;
	}

	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (lastState != (GuitarStates)itemState)
		{
			angleSnapped = false;
			positionSnapped = false;
		}
		if (itemState == ItemStates.State0)
		{
			Vector3 positionTarget = ((currentState == PositionState.InLeftHand) ? startPositionLeft : startPositionRight);
			Quaternion rotationTarget = ((currentState == PositionState.InLeftHand) ? startQuatLeft : startQuatRight);
			UpdateNonPlayingPosition(positionTarget, rotationTarget);
		}
		else if (itemState == ItemStates.State1)
		{
			Vector3 positionTarget2 = ((currentState == PositionState.InLeftHand) ? reverseGripPositionLeft : reverseGripPositionRight);
			Quaternion rotationTarget2 = ((currentState == PositionState.InLeftHand) ? reverseGripQuatLeft : reverseGripQuatRight);
			UpdateNonPlayingPosition(positionTarget2, rotationTarget2);
			if (IsMyItem() && (chestTouch.transform.position - currentChestCollider.transform.position).magnitude < snapDistance)
			{
				itemState = ItemStates.State2;
				angleSnapped = false;
				positionSnapped = false;
				currentChestCollider.gameObject.SetActive(value: true);
			}
		}
		else if (itemState == ItemStates.State2)
		{
			Quaternion quaternion = ((currentState == PositionState.InLeftHand) ? holdingOffsetRotationLeft : holdingOffsetRotationRight);
			Vector3 vector = ((currentState == PositionState.InLeftHand) ? chestOffsetLeft : chestOffsetRight);
			Quaternion quaternion2 = Quaternion.LookRotation(parentHand.position - currentChestCollider.transform.position) * quaternion;
			if (!angleSnapped && Quaternion.Angle(transform.rotation, quaternion2) > angleLerpSnap)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, quaternion2, lerpValue);
			}
			else
			{
				angleSnapped = true;
				transform.rotation = quaternion2;
			}
			Vector3 vector2 = currentChestCollider.transform.position + transform.rotation * vector;
			if (!positionSnapped && (transform.position - vector2).magnitude > vectorLerpSnap)
			{
				transform.position = Vector3.Lerp(transform.position, currentChestCollider.transform.position + transform.rotation * vector, lerpValue);
			}
			else
			{
				positionSnapped = true;
				transform.position = vector2;
			}
			if (currentState == PositionState.InRightHand)
			{
				parentHand = parentHandRight;
			}
			else
			{
				parentHand = parentHandLeft;
			}
			if (IsMyItem())
			{
				unsnapDistance = startingUnsnapDistance * myRig.transform.localScale.x;
				if (currentState == PositionState.InRightHand)
				{
					chestOffsetRight = Vector3.Scale(startingRightChestOffset, myRig.transform.localScale);
					currentChestCollider = chestColliderRight;
					fretHandIndicator = rightHandIndicator;
					strumHandIndicator = leftHandIndicator;
				}
				else
				{
					chestOffsetLeft = Vector3.Scale(startingLeftChestOffset, myRig.transform.localScale);
					currentChestCollider = chestColliderLeft;
					fretHandIndicator = leftHandIndicator;
					strumHandIndicator = rightHandIndicator;
				}
				if (Unsnap())
				{
					itemState = ItemStates.State1;
					angleSnapped = false;
					positionSnapped = false;
					if (currentState == PositionState.InLeftHand)
					{
						EquipmentInteractor.instance.wasLeftGrabPressed = true;
					}
					else
					{
						EquipmentInteractor.instance.wasRightGrabPressed = true;
					}
					currentChestCollider.gameObject.SetActive(value: false);
				}
				else
				{
					if (!handIn)
					{
						CheckFretFinger(fretHandIndicator.transform);
						HitChecker.CheckHandHit(ref collidersHitCount, interactableMask, sphereRadius, ref nullHit, ref raycastHits, ref raycastHitList, ref spherecastSweep, ref strumHandIndicator);
						if (collidersHitCount > 0)
						{
							for (int i = 0; i < collidersHitCount; i++)
							{
								if (raycastHits[i].collider != null && strumCollider == raycastHits[i].collider)
								{
									GorillaTagger.Instance.StartVibration(strumHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
									PlayNote(currentFretIndex, Mathf.Max(Mathf.Min(1f, strumHandIndicator.currentVelocity.magnitude / maxVelocity) * maxVolume, minVolume));
									if (NetworkSystem.Instance.InRoom && selfInstrumentIndex > -1)
									{
										GorillaTagger.Instance.myVRRig?.SendRPC("RPC_PlaySelfOnlyInstrument", RpcTarget.Others, selfInstrumentIndex, currentFretIndex, audioSource.volume);
									}
									break;
								}
							}
						}
					}
					handIn = HitChecker.CheckHandIn(ref anyHit, ref collidersHit, sphereRadius * transform.lossyScale.x, interactableMask, ref strumHandIndicator, ref strumList);
				}
			}
		}
		lastState = (GuitarStates)itemState;
	}

	public override void PlayNote(int note, float volume)
	{
		audioSource.time = 0.005f;
		audioSource.clip = audioClips[note];
		audioSource.volume = volume;
		audioSource.GTPlay();
		base.PlayNote(note, volume);
	}

	private bool Unsnap()
	{
		return (parentHand.position - chestTouch.position).magnitude > unsnapDistance;
	}

	private void CheckFretFinger(Transform finger)
	{
		for (int i = 0; i < collidersHit.Length; i++)
		{
			collidersHit[i] = null;
		}
		collidersHitCount = Physics.OverlapSphereNonAlloc(finger.position, sphereRadius, collidersHit, interactableMask, QueryTriggerInteraction.Collide);
		currentFretIndex = 5;
		if (collidersHitCount > 0)
		{
			for (int j = 0; j < collidersHit.Length; j++)
			{
				if (fretsList.Contains(collidersHit[j]))
				{
					currentFretIndex = fretsList.IndexOf(collidersHit[j]);
					if (currentFretIndex != lastFretIndex)
					{
						GorillaTagger.Instance.StartVibration(fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
					}
					lastFretIndex = currentFretIndex;
					break;
				}
			}
		}
		else
		{
			if (lastFretIndex != -1)
			{
				GorillaTagger.Instance.StartVibration(fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
			}
			lastFretIndex = -1;
		}
	}

	public void UpdateNonPlayingPosition(Vector3 positionTarget, Quaternion rotationTarget)
	{
		if (!angleSnapped)
		{
			if (Quaternion.Angle(rotationTarget, transform.localRotation) < angleLerpSnap)
			{
				angleSnapped = true;
				transform.localRotation = rotationTarget;
			}
			else
			{
				transform.localRotation = Quaternion.Slerp(transform.localRotation, rotationTarget, lerpValue);
			}
		}
		if (!positionSnapped)
		{
			if ((transform.localPosition - positionTarget).magnitude < vectorLerpSnap)
			{
				positionSnapped = true;
				transform.localPosition = positionTarget;
			}
			else
			{
				transform.localPosition = Vector3.Lerp(transform.localPosition, positionTarget, lerpValue);
			}
		}
	}

	public override bool CanDeactivate()
	{
		if (gameObject.activeSelf && itemState != ItemStates.State0)
		{
			return itemState == ItemStates.State1;
		}
		return true;
	}

	public override bool CanActivate()
	{
		if (itemState != ItemStates.State0)
		{
			return itemState == ItemStates.State1;
		}
		return true;
	}

	public override void OnActivate()
	{
		base.OnActivate();
		if (itemState == ItemStates.State0)
		{
			itemState = ItemStates.State1;
		}
		else
		{
			itemState = ItemStates.State0;
		}
	}

	public void GenerateVectorOffsetLeft()
	{
		chestOffsetLeft = transform.position - chestColliderLeft.transform.position;
		holdingOffsetRotationLeft = Quaternion.LookRotation(transform.position - chestColliderLeft.transform.position);
	}

	public void GenerateVectorOffsetRight()
	{
		chestOffsetRight = transform.position - chestColliderRight.transform.position;
		holdingOffsetRotationRight = Quaternion.LookRotation(transform.position - chestColliderRight.transform.position);
	}

	public void GenerateReverseGripOffsetLeft()
	{
		reverseGripPositionLeft = transform.localPosition;
		reverseGripQuatLeft = transform.localRotation;
	}

	public void GenerateClubOffsetLeft()
	{
		startPositionLeft = transform.localPosition;
		startQuatLeft = transform.localRotation;
	}

	public void GenerateReverseGripOffsetRight()
	{
		reverseGripPositionRight = transform.localPosition;
		reverseGripQuatRight = transform.localRotation;
	}

	public void GenerateClubOffsetRight()
	{
		startPositionRight = transform.localPosition;
		startQuatRight = transform.localRotation;
	}

	public void TestClubPositionRight()
	{
		transform.localPosition = startPositionRight;
		transform.localRotation = startQuatRight;
	}

	public void TestReverseGripPositionRight()
	{
		transform.localPosition = reverseGripPositionRight;
		transform.localRotation = reverseGripQuatRight;
	}

	public void TestPlayingPositionRight()
	{
		transform.rotation = Quaternion.LookRotation(parentHand.position - currentChestCollider.transform.position) * holdingOffsetRotationRight;
		transform.position = chestColliderRight.transform.position + transform.rotation * chestOffsetRight;
	}
}
