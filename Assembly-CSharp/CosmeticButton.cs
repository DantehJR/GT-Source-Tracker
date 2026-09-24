using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

public class CosmeticButton : GorillaPressableButton
{
	[SerializeField]
	private Vector3 pressedOffset = new Vector3(0f, 0f, 0.1f);

	[SerializeField]
	private Material disabledMaterial;

	[SerializeField]
	private Vector3 disabledOffset = new Vector3(0f, 0f, 0.1f);

	private Vector3 startingPos;

	protected Vector3 posOffset;

	public string SetCosmeticItemID;

	public bool Initialized { get; private set; }

	public void Awake()
	{
		startingPos = transform.localPosition;
		Initialized = true;
	}

	public override void UpdateColor()
	{
		if (isSubscriberOnlyButton && !_localPlayerSubscribed)
		{
			SetUnsubscribedMaterial();
			SetOffText(myText.IsNotNull(), myTmpText.IsNotNull(), myTmpText2.IsNotNull());
			return;
		}
		if (!enabled)
		{
			buttonRenderer.material = disabledMaterial;
			SetOffText(myText != null);
		}
		else if (isOn)
		{
			buttonRenderer.material = pressedMaterial;
			SetOnText(myText.IsNotNull());
		}
		else
		{
			buttonRenderer.material = unpressedMaterial;
			SetOffText(myText != null);
		}
		UpdatePosition();
	}

	public virtual void UpdatePosition()
	{
		Vector3 localPosition = startingPos;
		if (!enabled)
		{
			localPosition += disabledOffset;
		}
		else if (isOn)
		{
			localPosition += pressedOffset;
		}
		posOffset = transform.position;
		transform.localPosition = localPosition;
		posOffset = transform.position - posOffset;
		if (myText != null)
		{
			myText.transform.position += posOffset;
		}
		if (myTmpText != null)
		{
			myTmpText.transform.position += posOffset;
		}
		if (myTmpText2 != null)
		{
			myTmpText2.transform.position += posOffset;
		}
	}

	protected override bool AllowNonSubscribedPress()
	{
		if (CosmeticsController.instance == null || string.IsNullOrEmpty(SetCosmeticItemID) || !CosmeticsController.instance.currentWornSet.HasItem(SetCosmeticItemID))
		{
			return false;
		}
		return true;
	}
}
