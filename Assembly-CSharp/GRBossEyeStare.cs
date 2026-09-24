using System.Collections.Generic;
using UnityEngine;

public class GRBossEyeStare : MonoBehaviour, IGorillaSliceableSimple
{
	private Vector3 lastLocalRot;

	private List<GRAbilityBase> noUpdateAbilities = new List<GRAbilityBase>();

	private GREnemyBossMoon boss;

	private GRAbilityBase lastAbility;

	private float lastCheck;

	private float checkForClosestPlayerCooldown = 1f;

	private Transform closestPlayer;

	private List<VRRig> rigs = new List<VRRig>();

	public float lerpAmount = 0.3f;

	public Vector3 rotOffset;

	private void Awake()
	{
		boss = GetComponentInParent<GREnemyBossMoon>();
	}

	private void OnEnable()
	{
		lastLocalRot = transform.localEulerAngles;
		GorillaSlicerSimpleManager.RegisterSliceable(this);
	}

	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this);
	}

	public void SliceUpdate()
	{
		if (boss.CurrAbility != lastAbility)
		{
			lastLocalRot = transform.localEulerAngles;
		}
		if (noUpdateAbilities.Contains(boss.CurrAbility))
		{
			lastLocalRot = transform.localEulerAngles;
			lastAbility = boss.CurrAbility;
			return;
		}
		if (transform.localEulerAngles != lastLocalRot)
		{
			lastLocalRot = transform.localEulerAngles;
			if (!noUpdateAbilities.Contains(boss.CurrAbility))
			{
				noUpdateAbilities.Add(boss.CurrAbility);
			}
			lastAbility = boss.CurrAbility;
			return;
		}
		if (closestPlayer == null || Time.time > lastCheck + checkForClosestPlayerCooldown)
		{
			VRRigCache.Instance.GetActiveRigs(rigs);
			float num = float.MaxValue;
			for (int i = 0; i < rigs.Count; i++)
			{
				float sqrMagnitude = (transform.position - rigs[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					closestPlayer = rigs[i].transform;
				}
			}
			lastCheck = Time.time;
		}
		lastAbility = boss.CurrAbility;
		if (!(closestPlayer == null))
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.up, (closestPlayer.position - transform.position).normalized) * Quaternion.Euler(rotOffset), lerpAmount);
			lastLocalRot = transform.localEulerAngles;
		}
	}
}
