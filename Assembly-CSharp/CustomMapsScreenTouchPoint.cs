using System.Collections;
using GorillaTag;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using UnityEngine;

public abstract class CustomMapsScreenTouchPoint : MonoBehaviour, IClickable
{
	public enum TouchPointDirections
	{
		Forward,
		Backward,
		Left,
		Right,
		Up,
		Down
	}

	[SerializeField]
	private CustomMapsTerminalScreen screen;

	[SerializeField]
	private CustomMapKeyboardBinding keyBinding;

	[SerializeField]
	private TouchPointDirections forwardDirection;

	[SerializeField]
	protected SpriteRenderer touchPointRenderer;

	[SerializeField]
	protected ButtonColorSettings buttonColorSettings;

	private static float pressedTime = 0.25f;

	protected static float pressTime;

	private Coroutine colorUpdateCoroutine;

	protected virtual void Awake()
	{
	}

	protected virtual void OnDisable()
	{
		if (colorUpdateCoroutine != null)
		{
			StopCoroutine(colorUpdateCoroutine);
		}
		if (buttonColorSettings != null)
		{
			touchPointRenderer.color = buttonColorSettings.UnpressedColor;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		GTDev.Log($"trigger {gameObject.name} pressTime={pressTime} time={Time.time}");
		if (Time.time < pressTime + pressedTime || !(collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null))
		{
			return;
		}
		Vector3 rhs = GetForwardDirection();
		if (!(Vector3.Dot((collider.transform.position - transform.position).normalized, rhs) < 0f))
		{
			GTDev.Log($"trigger {gameObject.name} collider {collider.gameObject.name} postion {collider.transform.position}");
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			pressTime = Time.time;
			OnButtonPressedEvent();
			PressButtonColourUpdate();
			if (screen != null)
			{
				screen.PressButton(keyBinding);
			}
			if (component != null)
			{
				GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			}
		}
	}

	public virtual void PressButtonColourUpdate()
	{
		if (gameObject.activeInHierarchy)
		{
			touchPointRenderer.color = buttonColorSettings.PressedColor;
			colorUpdateCoroutine = StartCoroutine(ButtonColorUpdate_Local());
		}
		IEnumerator ButtonColorUpdate_Local()
		{
			yield return new WaitForSeconds(pressedTime);
			if (pressTime != 0f && Time.time > pressedTime + pressTime)
			{
				touchPointRenderer.color = buttonColorSettings.UnpressedColor;
				pressTime = 0f;
			}
		}
	}

	private Vector3 GetForwardDirection()
	{
		return forwardDirection switch
		{
			TouchPointDirections.Forward => transform.forward, 
			TouchPointDirections.Backward => -transform.forward, 
			TouchPointDirections.Left => -transform.right, 
			TouchPointDirections.Right => transform.right, 
			TouchPointDirections.Up => transform.up, 
			TouchPointDirections.Down => -transform.up, 
			_ => transform.forward, 
		};
	}

	protected abstract void OnButtonPressedEvent();

	public void Click(bool leftHand = false)
	{
	}
}
