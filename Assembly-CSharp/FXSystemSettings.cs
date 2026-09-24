using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/FXSystemSettings", order = 2)]
public class FXSystemSettings : ScriptableObject
{
	private const string preLog = "FXSystemSettings: ";

	private const string preErr = "ERROR!!!  FXSystemSettings: ";

	[SerializeField]
	private LimiterType[] callLimits;

	[SerializeField]
	private CooldownType[] CallLimitsCooldown;

	[NonSerialized]
	public bool forLocalRig;

	[NonSerialized]
	public CallLimitType<CallLimiter>[] callSettings = new CallLimitType<CallLimiter>[27];

	public void Awake()
	{
		int num = ((callLimits != null) ? callLimits.Length : 0);
		int num2 = ((CallLimitsCooldown != null) ? CallLimitsCooldown.Length : 0);
		for (int i = 0; i < num; i++)
		{
			FXType key = callLimits[i].Key;
			int num3 = (int)key;
			if (num3 < 0 || num3 >= 27)
			{
				string text = "NO_PATH_AT_RUNTIME";
				Debug.LogError("FXSystemSettings: (this should never happen) `callLimits.Key` is out of bounds of `callSettings`! Path=\"" + text + "\"", this);
			}
			if (callSettings[num3] != null)
			{
				Debug.Log("FXSystemSettings: call setting for " + key.ToString() + " already exists, skipping.");
			}
			else
			{
				callSettings[num3] = callLimits[i];
			}
		}
		for (int i = 0; i < num2; i++)
		{
			FXType key = CallLimitsCooldown[i].Key;
			int num3 = (int)key;
			if (callSettings[num3] != null)
			{
				Debug.Log("FXSystemSettings: call setting for " + key.ToString() + " already exists, skipping");
			}
			else
			{
				callSettings[num3] = CallLimitsCooldown[i];
			}
		}
		for (int i = 0; i < callSettings.Length; i++)
		{
			if (callSettings[i] == null)
			{
				callSettings[i] = new LimiterType
				{
					CallLimitSettings = new CallLimiter(0, 0f, 0f),
					Key = (FXType)i
				};
			}
		}
	}
}
