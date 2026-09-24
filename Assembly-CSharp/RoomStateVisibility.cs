using System;
using UnityEngine;

public class RoomStateVisibility : MonoBehaviour
{
	[SerializeField]
	private bool enableOutOfRoom;

	[SerializeField]
	private bool enableInRoom = true;

	[SerializeField]
	private bool enableInPrivateRoom = true;

	private void Start()
	{
		OnRoomChanged();
		RoomSystem.JoinedRoomEvent += new Action(OnRoomChanged);
		RoomSystem.LeftRoomEvent += new Action(OnRoomChanged);
	}

	private void OnDestroy()
	{
		RoomSystem.JoinedRoomEvent -= new Action(OnRoomChanged);
		RoomSystem.LeftRoomEvent -= new Action(OnRoomChanged);
	}

	private void OnRoomChanged()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				gameObject.SetActive(enableInPrivateRoom);
			}
			else
			{
				gameObject.SetActive(enableInRoom);
			}
		}
		else
		{
			gameObject.SetActive(enableOutOfRoom);
		}
	}
}
