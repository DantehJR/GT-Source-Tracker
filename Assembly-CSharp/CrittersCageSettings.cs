using UnityEngine;

public class CrittersCageSettings : CrittersActorSettings
{
	public Transform cagePoint;

	public Transform grabPoint;

	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersCage crittersCage = (CrittersCage)parentActor;
		crittersCage.cagePosition = cagePoint;
		crittersCage.grabPosition = grabPoint;
	}
}
