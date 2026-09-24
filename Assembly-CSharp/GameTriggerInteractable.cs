using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameEntity))]
public class GameTriggerInteractable : MonoBehaviour
{
	public GameEntity gameEntity;

	public Transform interactableCenter;

	public float interactableRadius;

	public bool interactableWhileGrabbed;

	public bool interactableWhileSnapped;

	public bool interactablePermanently;

	public bool interactableOnOthers;

	public bool triggerInteractionActive;

	public int handIndex = -1;

	public static List<GameTriggerInteractable> LocalInteractableTriggers = new List<GameTriggerInteractable>();

	private void OnEnable()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = GetComponent<GameEntity>();
		}
		if (interactableWhileGrabbed)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(StartHolding));
			GameEntity gameEntity2 = this.gameEntity;
			gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(StopHolding));
		}
		if (interactableWhileSnapped)
		{
			GameEntity gameEntity3 = this.gameEntity;
			gameEntity3.OnSnapped = (Action)Delegate.Combine(gameEntity3.OnSnapped, new Action(StartHolding));
			GameEntity gameEntity4 = this.gameEntity;
			gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(StopHolding));
		}
	}

	public void StartHolding()
	{
		LocalInteractableTriggers.AddIfNew(this);
	}

	public void StopHolding()
	{
		LocalInteractableTriggers.RemoveIfContains(this);
	}

	public bool PointWithinInteractableArea(Vector3 point)
	{
		return (interactableCenter.position - point).magnitude < interactableRadius;
	}

	public void BeginTriggerInteraction(int _handIndex)
	{
		triggerInteractionActive = true;
		handIndex = _handIndex;
	}

	public void EndTriggerInteraction()
	{
		triggerInteractionActive = false;
		handIndex = -1;
	}
}
