using System;

namespace Photon.Voice.Unity;

public class RemoteVoiceLink : IEquatable<RemoteVoiceLink>
{
	public readonly VoiceInfo Info;

	public readonly int PlayerId;

	public readonly int VoiceId;

	public readonly int ChannelId;

	private string cached;

	public event Action<FrameOut<float>> FloatFrameDecoded;

	public event Action RemoteVoiceRemoved;

	public RemoteVoiceLink(VoiceInfo info, int playerId, int voiceId, int channelId)
	{
		Info = info;
		PlayerId = playerId;
		VoiceId = voiceId;
		ChannelId = channelId;
	}

	public void Init(ref RemoteVoiceOptions options)
	{
		options.SetOutput(OnDecodedFrameFloatAction);
		options.OnRemoteVoiceRemoveAction = OnRemoteVoiceRemoveAction;
	}

	private void OnRemoteVoiceRemoveAction()
	{
		if (RemoteVoiceRemoved != null)
		{
			RemoteVoiceRemoved();
		}
	}

	private void OnDecodedFrameFloatAction(FrameOut<float> floats)
	{
		if (FloatFrameDecoded != null)
		{
			FloatFrameDecoded(floats);
		}
	}

	public override string ToString()
	{
		if (string.IsNullOrEmpty(cached))
		{
			cached = $"[p#:{PlayerId},v#:{VoiceId},c#:{ChannelId},i:{{{Info}}}]";
		}
		return cached;
	}

	public bool Equals(RemoteVoiceLink other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (PlayerId != other.PlayerId || VoiceId != other.VoiceId)
		{
			return Info.UserData == other.Info.UserData;
		}
		return true;
	}
}
