using GorillaTag;
using UnityEngine;

public class ZoneRootRegister : MonoBehaviour
{
	[SerializeField]
	private WatchableGameObjectSO watchableSlot;

	private void Awake()
	{
		watchableSlot.Value = gameObject;
	}

	private void OnDestroy()
	{
		watchableSlot.Value = null;
	}
}
