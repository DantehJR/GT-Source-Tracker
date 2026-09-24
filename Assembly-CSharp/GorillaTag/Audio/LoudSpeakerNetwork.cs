using System.Collections.Generic;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio;

public class LoudSpeakerNetwork : MonoBehaviour
{
	[SerializeField]
	private AudioSource[] _speakerSources;

	[SerializeField]
	private List<Speaker> _currentSpeakers;

	[SerializeField]
	private int _currentSpeakerActor = -1;

	public bool ReparentLocalSpeaker = true;

	private RigContainer _rigContainer;

	private GTRecorder _localRecorder;

	public AudioSource[] SpeakerSources => _speakerSources;

	private void Awake()
	{
		if (_speakerSources == null || _speakerSources.Length == 0)
		{
			_speakerSources = transform.GetComponentsInChildren<AudioSource>();
		}
		_currentSpeakers = new List<Speaker>();
	}

	private void Start()
	{
		if (GetParentRigContainer(out var rigContainer) && rigContainer.Voice != null)
		{
			GTSpeaker gTSpeaker = (GTSpeaker)rigContainer.Voice.SpeakerInUse;
			if (gTSpeaker != null)
			{
				gTSpeaker.AddExternalAudioSources(_speakerSources);
			}
		}
	}

	private bool GetParentRigContainer(out RigContainer rigContainer)
	{
		if (_rigContainer == null)
		{
			_rigContainer = transform.GetComponentInParent<RigContainer>();
		}
		rigContainer = _rigContainer;
		return rigContainer != null;
	}

	private void OnEnable()
	{
		if (GetParentRigContainer(out var rigContainer))
		{
			rigContainer.AddLoudSpeakerNetwork(this);
		}
	}

	private void OnDisable()
	{
		if (GetParentRigContainer(out var rigContainer))
		{
			rigContainer.RemoveLoudSpeakerNetwork(this);
		}
	}

	public void AddSpeaker(Speaker speaker)
	{
		if (!_currentSpeakers.Contains(speaker))
		{
			_currentSpeakers.Add(speaker);
		}
	}

	public void RemoveSpeaker(Speaker speaker)
	{
		_currentSpeakers.Remove(speaker);
	}

	public void StartBroadcastSpeakerOutput(VRRig player)
	{
		GorillaTagger.Instance.rigSerializer.BroadcastLoudSpeakerNetwork(toggleBroadcast: true, player.OwningNetPlayer.ActorNumber);
	}

	public void BroadcastLoudSpeakerNetwork(int actorNumber, bool isLocal = false)
	{
		if (isLocal)
		{
			if (_localRecorder == null)
			{
				_localRecorder = (GTRecorder)NetworkSystem.Instance.LocalRecorder;
			}
			if (_localRecorder != null)
			{
				_localRecorder.DebugEchoMode = true;
				if (ReparentLocalSpeaker)
				{
					Transform transform = _rigContainer.Voice.SpeakerInUse.transform;
					transform.transform.SetParent(base.transform, worldPositionStays: false);
					transform.localPosition = Vector3.zero;
				}
			}
			return;
		}
		using (List<Speaker>.Enumerator enumerator = _currentSpeakers.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				GTSpeaker gTSpeaker = (GTSpeaker)enumerator.Current;
				gTSpeaker.ToggleAudioSource(toggle: true);
				gTSpeaker.BroadcastExternal = true;
				if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out var playerRig))
				{
					Transform transform2 = playerRig.Voice.SpeakerInUse.transform;
					transform2.SetParent(base.transform, worldPositionStays: false);
					transform2.localPosition = Vector3.zero;
				}
			}
		}
		_currentSpeakerActor = actorNumber;
	}

	public void StopBroadcastSpeakerOutput(VRRig player)
	{
		GorillaTagger.Instance.rigSerializer.BroadcastLoudSpeakerNetwork(toggleBroadcast: false, player.OwningNetPlayer.ActorNumber);
	}

	public void StopBroadcastLoudSpeakerNetwork(int actorNumber, bool isLocal = false)
	{
		if (isLocal)
		{
			if (_localRecorder == null)
			{
				_localRecorder = (GTRecorder)NetworkSystem.Instance.LocalRecorder;
			}
			if (_localRecorder != null)
			{
				_localRecorder.DebugEchoMode = false;
				if (ReparentLocalSpeaker && GetParentRigContainer(out var rigContainer))
				{
					Transform transform = rigContainer.Voice.SpeakerInUse.transform;
					transform.SetParent(rigContainer.SpeakerHead, worldPositionStays: false);
					transform.localPosition = Vector3.zero;
				}
			}
		}
		else
		{
			if (actorNumber != _currentSpeakerActor)
			{
				return;
			}
			using (List<Speaker>.Enumerator enumerator = _currentSpeakers.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					GTSpeaker gTSpeaker = (GTSpeaker)enumerator.Current;
					gTSpeaker.ToggleAudioSource(toggle: false);
					gTSpeaker.BroadcastExternal = false;
					if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out var playerRig))
					{
						Transform transform2 = playerRig.Voice.SpeakerInUse.transform;
						transform2.SetParent(playerRig.SpeakerHead, worldPositionStays: false);
						transform2.localPosition = Vector3.zero;
					}
				}
			}
			_currentSpeakerActor = -1;
		}
	}
}
