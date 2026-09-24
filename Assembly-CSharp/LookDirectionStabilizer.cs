using GorillaTag;
using GorillaTag.CosmeticSystem;
using Unity.Cinemachine;
using UnityEngine;

public class LookDirectionStabilizer : MonoBehaviour, ISpawnable
{
	private VRRig myRig;

	public bool IsSpawned { get; set; }

	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	void ISpawnable.OnSpawn(VRRig rig)
	{
		myRig = rig;
	}

	void ISpawnable.OnDespawn()
	{
	}

	private void Update()
	{
		Transform rigTarget = myRig.head.rigTarget;
		Vector3 up = myRig.transform.up;
		if (Vector3.Dot(rigTarget.forward, up) < 0f)
		{
			Quaternion b = Quaternion.LookRotation(rigTarget.up.ProjectOntoPlane(up), up);
			Quaternion rotation = transform.parent.rotation;
			float value = Vector3.Dot(rigTarget.up, up);
			transform.rotation = Quaternion.Lerp(rotation, b, Mathf.InverseLerp(1f, 0.7f, value));
		}
		else
		{
			transform.localRotation = Quaternion.identity;
		}
	}
}
