using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

public class ChestObjectHysteresis : MonoBehaviour, ISpawnable
{
	public float angleHysteresis;

	public float angleBetween;

	public Transform angleFollower;

	[Delayed]
	public string angleFollower_path;

	private Quaternion lastAngleQuat;

	private Quaternion currentAngleQuat;

	bool ISpawnable.IsSpawned { get; set; }

	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	void ISpawnable.OnSpawn(VRRig rig)
	{
		if (!angleFollower && (string.IsNullOrEmpty(angleFollower_path) || transform.TryFindByPath(angleFollower_path, out angleFollower)))
		{
			Debug.LogError("ChestObjectHysteresis: DEACTIVATING! Could not find `angleFollower` using path: \"" + angleFollower_path + "\". For component at: \"" + this.GetComponentPath() + "\"", this);
			gameObject.SetActive(value: false);
		}
	}

	void ISpawnable.OnDespawn()
	{
	}

	private void Start()
	{
		lastAngleQuat = transform.rotation;
		currentAngleQuat = transform.rotation;
	}

	private void OnEnable()
	{
		ChestObjectHysteresisManager.RegisterCH(this);
	}

	private void OnDisable()
	{
		ChestObjectHysteresisManager.UnregisterCH(this);
	}

	public void InvokeUpdate()
	{
		currentAngleQuat = angleFollower.rotation;
		angleBetween = Quaternion.Angle(currentAngleQuat, lastAngleQuat);
		if (angleBetween > angleHysteresis)
		{
			transform.rotation = Quaternion.Slerp(currentAngleQuat, lastAngleQuat, angleHysteresis / angleBetween);
			lastAngleQuat = transform.rotation;
		}
		transform.rotation = lastAngleQuat;
	}
}
