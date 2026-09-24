public class CrittersAttachPointSettings : CrittersActorSettings
{
	public bool isLeft;

	public CrittersAttachPoint.AnchoredLocationTypes anchoredLocation;

	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersAttachPoint crittersAttachPoint = (CrittersAttachPoint)parentActor;
		crittersAttachPoint.anchorLocation = anchoredLocation;
		crittersAttachPoint.rb.isKinematic = true;
		crittersAttachPoint.isLeft = isLeft;
	}
}
