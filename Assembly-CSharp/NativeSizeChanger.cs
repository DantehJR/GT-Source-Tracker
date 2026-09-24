using GorillaLocomotion;
using UnityEngine;

public class NativeSizeChanger : MonoBehaviour
{
	public void Activate(NativeSizeChangerSettings settings)
	{
		settings.WorldPosition = transform.position;
		settings.ActivationTime = Time.time;
		GTPlayer.Instance.SetNativeScale(settings);
	}
}
