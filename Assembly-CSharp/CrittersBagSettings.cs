using System.Collections.Generic;
using UnityEngine;

public class CrittersBagSettings : CrittersActorSettings
{
	public Collider attachableCollider;

	public BoxCollider dropCube;

	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	public List<Collider> attachDisableColliders;

	public AudioClip attachSound;

	public AudioClip detachSound;

	public List<CrittersActor.CrittersActorType> blockAttachTypes;

	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersBag crittersBag = (CrittersBag)parentActor;
		crittersBag.attachableCollider = attachableCollider;
		crittersBag.dropCube = dropCube;
		crittersBag.anchorLocation = anchorLocation;
		crittersBag.attachDisableColliders = attachDisableColliders;
		crittersBag.attachSound = attachSound;
		crittersBag.detachSound = detachSound;
		crittersBag.blockAttachTypes = blockAttachTypes;
	}
}
