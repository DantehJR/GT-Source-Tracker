using UnityEngine;

public class GorillaSetZoneTrigger : GorillaTriggerBox
{
	[SerializeField]
	private GTZone[] zones;

	public override void OnBoxTriggered()
	{
		Debug.Log("Triggered set zone box on gameobject " + gameObject.name);
		ZoneManagement.SetActiveZones(zones);
	}
}
