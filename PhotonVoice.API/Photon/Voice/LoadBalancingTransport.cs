using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Voice;

public class LoadBalancingTransport : LoadBalancingClient, IVoiceTransport, ILogger, IDisposable
{
	internal const int VOICE_CHANNEL = 0;

	protected VoiceClient voiceClient;

	private PhotonTransportProtocol protocol;

	public VoiceClient VoiceClient => voiceClient;

	[Obsolete("Use GlobalInterestGroup.")]
	public byte GlobalAudioGroup
	{
		get
		{
			return GlobalInterestGroup;
		}
		set
		{
			GlobalInterestGroup = value;
		}
	}

	public byte GlobalInterestGroup
	{
		get
		{
			return voiceClient.GlobalInterestGroup;
		}
		set
		{
			voiceClient.GlobalInterestGroup = value;
			if (State == ClientState.Joined)
			{
				if (voiceClient.GlobalInterestGroup != 0)
				{
					LoadBalancingPeer.OpChangeGroups(new byte[0], new byte[1] { voiceClient.GlobalInterestGroup });
				}
				else
				{
					LoadBalancingPeer.OpChangeGroups(new byte[0], null);
				}
			}
		}
	}

	public void LogError(string fmt, params object[] args)
	{
		DebugReturn(DebugLevel.ERROR, string.Format(fmt, args));
	}

	public void LogWarning(string fmt, params object[] args)
	{
		DebugReturn(DebugLevel.WARNING, string.Format(fmt, args));
	}

	public void LogInfo(string fmt, params object[] args)
	{
		DebugReturn(DebugLevel.INFO, string.Format(fmt, args));
	}

	public void LogDebug(string fmt, params object[] args)
	{
		DebugReturn(DebugLevel.ALL, string.Format(fmt, args));
	}

	internal byte photonChannelForCodec(Codec c)
	{
		return (byte)(1 + Array.IndexOf(Enum.GetValues(typeof(Codec)), c));
	}

	public bool IsChannelJoined(int channelId)
	{
		return State == ClientState.Joined;
	}

	public LoadBalancingTransport(ILogger logger = null, ConnectionProtocol connectionProtocol = ConnectionProtocol.Udp)
		: base(connectionProtocol)
	{
		if (logger == null)
		{
			logger = this;
		}
		EventReceived += onEventActionVoiceClient;
		StateChanged += onStateChangeVoiceClient;
		voiceClient = new VoiceClient(this, logger);
		int num = Enum.GetValues(typeof(Codec)).Length + 1;
		if (LoadBalancingPeer.ChannelCount < num)
		{
			LoadBalancingPeer.ChannelCount = (byte)num;
		}
		protocol = new PhotonTransportProtocol(voiceClient, logger);
	}

	public new void Service()
	{
		base.Service();
		voiceClient.Service();
	}

	[Obsolete("Use LoadBalancingPeer::OpChangeGroups().")]
	public virtual bool ChangeAudioGroups(byte[] groupsToRemove, byte[] groupsToAdd)
	{
		return LoadBalancingPeer.OpChangeGroups(groupsToRemove, groupsToAdd);
	}

	public void SendVoicesInfo(IEnumerable<LocalVoice> voices, int channelId, int targetPlayerId)
	{
		foreach (IGrouping<Codec, LocalVoice> item in from v in voices
			group v by v.Info.Codec)
		{
			object customEventContent = protocol.buildVoicesInfo(item, logInfo: true);
			SendOptions sendOptions = new SendOptions
			{
				Reliability = true,
				Channel = photonChannelForCodec(item.Key)
			};
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			switch (targetPlayerId)
			{
			case -1:
				raiseEventOptions.TargetActors = new int[1] { LocalPlayer.ActorNumber };
				break;
			default:
				raiseEventOptions.TargetActors = new int[1] { targetPlayerId };
				break;
			case 0:
				break;
			}
			OpRaiseEvent(202, customEventContent, raiseEventOptions, sendOptions);
		}
	}

	public void SendVoiceRemove(LocalVoice voice, int channelId, int targetPlayerId)
	{
		object customEventContent = protocol.buildVoiceRemoveMessage(voice);
		SendOptions sendOptions = new SendOptions
		{
			Reliability = true,
			Channel = photonChannelForCodec(voice.Info.Codec)
		};
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		switch (targetPlayerId)
		{
		case -1:
			raiseEventOptions.TargetActors = new int[1] { LocalPlayer.ActorNumber };
			break;
		default:
			raiseEventOptions.TargetActors = new int[1] { targetPlayerId };
			break;
		case 0:
			break;
		}
		if (voice.DebugEchoMode)
		{
			raiseEventOptions.Receivers = ReceiverGroup.All;
		}
		OpRaiseEvent(202, customEventContent, raiseEventOptions, sendOptions);
	}

	public virtual void SendFrame(ArraySegment<byte> data, FrameFlags flags, byte evNumber, byte voiceId, int channelId, int targetPlayerId, bool reliable, LocalVoice localVoice)
	{
		object[] customEventContent = protocol.buildFrameMessage(voiceId, evNumber, data, flags);
		SendOptions sendOptions = new SendOptions
		{
			Reliability = reliable,
			Channel = photonChannelForCodec(localVoice.Info.Codec),
			Encrypt = localVoice.Encrypt
		};
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		switch (targetPlayerId)
		{
		case -1:
			raiseEventOptions.TargetActors = new int[1] { LocalPlayer.ActorNumber };
			break;
		default:
			raiseEventOptions.TargetActors = new int[1] { targetPlayerId };
			break;
		case 0:
			break;
		}
		if (localVoice.DebugEchoMode)
		{
			raiseEventOptions.Receivers = ReceiverGroup.All;
		}
		raiseEventOptions.InterestGroup = localVoice.InterestGroup;
		OpRaiseEvent(202, customEventContent, raiseEventOptions, sendOptions);
		while (LoadBalancingPeer.SendOutgoingCommands())
		{
		}
	}

	public string ChannelIdStr(int channelId)
	{
		return null;
	}

	public string PlayerIdStr(int playerId)
	{
		return null;
	}

	protected virtual void onEventActionVoiceClient(EventData ev)
	{
		if (ev.Code == 202)
		{
			protocol.onVoiceEvent(ev[245], 0, ev.Sender, ev.Sender == LocalPlayer.ActorNumber);
			return;
		}
		switch (ev.Code)
		{
		case byte.MaxValue:
		{
			int sender = ev.Sender;
			if (sender != LocalPlayer.ActorNumber)
			{
				voiceClient.onPlayerJoin(0, sender);
			}
			break;
		}
		case 254:
		{
			int sender = ev.Sender;
			if (sender == LocalPlayer.ActorNumber)
			{
				voiceClient.onLeaveAllChannels();
			}
			else
			{
				voiceClient.onPlayerLeave(0, sender);
			}
			break;
		}
		}
	}

	private void onStateChangeVoiceClient(ClientState fromState, ClientState state)
	{
		if (fromState == ClientState.Joined)
		{
			voiceClient.onLeaveChannel(0);
		}
		if (state == ClientState.Joined)
		{
			voiceClient.onJoinChannel(0);
			if (voiceClient.GlobalInterestGroup != 0)
			{
				LoadBalancingPeer.OpChangeGroups(new byte[0], new byte[1] { voiceClient.GlobalInterestGroup });
			}
		}
	}

	public void Dispose()
	{
		voiceClient.Dispose();
	}
}
