using System;
using UnityEngine;

public class GRGameEntityInteractionPoint : MonoBehaviour
{
	public GameEntity gameEntity;

	public float autoReleaseDistance = 0.1f;

	public Action OnGrabStart;

	public Action OnGrabContinue;

	public Action OnGrabEnd;

	public Transform targetParent;

	public void Start()
	{
		transform.parent = targetParent;
	}

	public void OnEnable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(OnReleased));
	}

	public void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(OnReleased));
	}

	public void OnGrabbed()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(TickWhileHeld));
		OnGrabStart?.Invoke();
	}

	public void OnReleased()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Remove(gameEntity.OnTick, new Action(TickWhileHeld));
		this.gameEntity.transform.parent = targetParent;
		this.gameEntity.transform.localRotation = Quaternion.identity;
		this.gameEntity.transform.localPosition = Vector3.zero;
		OnGrabEnd();
	}

	public void TickWhileHeld()
	{
		if (targetParent != null)
		{
			Vector3 position = targetParent.transform.position;
			Vector3 position2 = transform.position;
			if (Vector3.Magnitude(position - position2) > autoReleaseDistance)
			{
				GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
				if (gamePlayer != null)
				{
					gamePlayer.ClearGrabbedIfHeld(gameEntity.id, gameEntity.manager);
				}
				if (gamePlayer != null && GamePlayerLocal.instance.gamePlayer == gamePlayer)
				{
					GamePlayerLocal.instance.ClearGrabbedIfHeld(gameEntity.id, gameEntity.manager);
				}
				OnReleased();
				return;
			}
		}
		OnGrabContinue?.Invoke();
	}
}
