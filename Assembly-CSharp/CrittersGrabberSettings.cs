using UnityEngine;

public class CrittersGrabberSettings : CrittersActorSettings
{
	public Transform _grabPosition;

	public float _grabDistance;

	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersGrabber crittersGrabber = (CrittersGrabber)parentActor;
		crittersGrabber.grabPosition = _grabPosition;
		crittersGrabber.grabDistance = _grabDistance;
	}
}
