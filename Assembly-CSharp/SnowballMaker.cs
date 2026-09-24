using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag;
using UnityEngine;

public class SnowballMaker : MonoBehaviourPostTick
{
	public bool isLeftHand;

	public GorillaVelocityEstimator velocityEstimator;

	private float snowballCreationCooldownTime = 0.1f;

	private float lastGroundContactTime;

	private bool requiresFreshMaterialContact;

	private Transform handTransform;

	private Dictionary<int, SnowballThrowable> matSnowballLookup = new Dictionary<int, SnowballThrowable>();

	private Dictionary<int, SnowballThrowable> snowballByThrowableIndex = new Dictionary<int, SnowballThrowable>();

	private Dictionary<int, string> snowballPlayfabIdByThrowableIndex = new Dictionary<int, string>();

	private Dictionary<int, string> snowballPlayfabIdByMaterialIndex = new Dictionary<int, string>();

	private bool m_hasGrowingSnowball;

	public static SnowballMaker leftHandInstance { get; private set; }

	public static SnowballMaker rightHandInstance { get; private set; }

	public SnowballThrowable[] snowballs { get; private set; }

	private void Awake()
	{
		if (snowballs == null)
		{
			snowballs = new SnowballThrowable[0];
		}
		SnowballThrowable[] array = snowballs;
		foreach (SnowballThrowable snowballThrowable in array)
		{
			if (!snowballThrowable.IsNull() && snowballThrowable is GrowingSnowballThrowable)
			{
				m_hasGrowingSnowball = true;
			}
		}
		if (isLeftHand)
		{
			if (leftHandInstance == null)
			{
				leftHandInstance = this;
			}
			else
			{
				Object.Destroy(gameObject);
			}
		}
		else if (rightHandInstance == null)
		{
			rightHandInstance = this;
		}
		else
		{
			Object.Destroy(gameObject);
		}
	}

	private void Start()
	{
		handTransform = (isLeftHand ? GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.leftHandTransform : GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.rightHandTransform);
	}

	internal void SetupThrowables(SnowballThrowable[] newThrowables)
	{
		snowballs = newThrowables;
		for (int i = 0; i < snowballs.Length; i++)
		{
			if (!(snowballs[i] == null))
			{
				for (int j = 0; j < snowballs[i].matDataIndexes.Count; j++)
				{
					matSnowballLookup.TryAdd(snowballs[i].matDataIndexes[j], snowballs[i]);
				}
			}
		}
	}

	public override void PostTick()
	{
		if (ApplicationQuittingState.IsQuitting || (BuilderPieceInteractor.instance != null && BuilderPieceInteractor.instance.BlockSnowballCreation()) || !GTPlayer.hasInstance || !EquipmentInteractor.hasInstance || !GorillaTagger.hasInstance || !GorillaTagger.Instance.offlineVRRig)
		{
			return;
		}
		int materialTouchIndex = GTPlayer.Instance.GetMaterialTouchIndex(isLeftHand);
		if (materialTouchIndex == 0)
		{
			if (Time.time > lastGroundContactTime + snowballCreationCooldownTime)
			{
				requiresFreshMaterialContact = false;
			}
			return;
		}
		lastGroundContactTime = Time.time;
		InitializeSnowballFromMatIndex(materialTouchIndex);
		EquipmentInteractor instance = EquipmentInteractor.instance;
		bool flag = (isLeftHand ? instance.leftHandHeldEquipment : instance.rightHandHeldEquipment) != null;
		bool flag2 = (isLeftHand ? instance.isLeftGrabbing : instance.isRightGrabbing);
		bool flag3 = (isLeftHand ? instance.disableLeftGrab : instance.disableRightGrab);
		bool flag4 = false;
		if (!flag2 || flag3 || requiresFreshMaterialContact)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < snowballs.Length; i++)
		{
			SnowballThrowable snowballThrowable = snowballs[i];
			if (!(snowballThrowable == null) && snowballThrowable.gameObject.activeSelf)
			{
				num = i;
				break;
			}
		}
		SnowballThrowable snowballThrowable2 = ((num > -1) ? snowballs[num] : null);
		GrowingSnowballThrowable growingSnowballThrowable = snowballThrowable2 as GrowingSnowballThrowable;
		bool flag5 = (isLeftHand ? (!ConnectedControllerHandler.Instance.RightValid) : (!ConnectedControllerHandler.Instance.LeftValid));
		SnowballThrowable value;
		if (growingSnowballThrowable != null && (!GrowingSnowballThrowable.twoHandedSnowballGrowing | flag5 | flag4))
		{
			if (snowballThrowable2.matDataIndexes.Contains(materialTouchIndex))
			{
				growingSnowballThrowable.IncreaseSize(1);
				GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				requiresFreshMaterialContact = true;
			}
		}
		else if (!flag && matSnowballLookup.TryGetValue(materialTouchIndex, out value))
		{
			Transform transform = value.transform;
			Transform transform2 = handTransform;
			XformOffset spawnOffset = value.SpawnOffset;
			value.SetSnowballActiveLocal(enabled: true);
			value.velocityEstimator = velocityEstimator;
			transform.position = transform2.TransformPoint(spawnOffset.pos);
			transform.rotation = transform2.rotation * spawnOffset.rot;
			GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			requiresFreshMaterialContact = true;
		}
	}

	public bool TryCreateSnowball(int materialIndex, out SnowballThrowable result)
	{
		EquipmentInteractor instance = EquipmentInteractor.instance;
		if (isLeftHand ? instance.disableLeftGrab : instance.disableRightGrab)
		{
			result = null;
			return false;
		}
		InitializeSnowballFromMatIndex(materialIndex);
		SnowballThrowable[] array = snowballs;
		foreach (SnowballThrowable snowballThrowable in array)
		{
			if (!(snowballThrowable == null) && snowballThrowable.matDataIndexes.Contains(materialIndex))
			{
				Transform transform = snowballThrowable.transform;
				Transform transform2 = handTransform;
				XformOffset spawnOffset = snowballThrowable.SpawnOffset;
				snowballThrowable.SetSnowballActiveLocal(enabled: true);
				snowballThrowable.velocityEstimator = velocityEstimator;
				transform.position = transform2.TransformPoint(spawnOffset.pos);
				transform.rotation = transform2.rotation * spawnOffset.rot;
				GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				result = snowballThrowable;
				return true;
			}
		}
		result = null;
		return false;
	}

	private void InitializeSnowballFromMatIndex(int matIndex)
	{
		if (CosmeticsV2Spawner_Dirty.GetThrowableIDFromMaterialIndex(isLeftHand, matIndex, out var throwableId))
		{
			VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(throwableId);
		}
	}
}
