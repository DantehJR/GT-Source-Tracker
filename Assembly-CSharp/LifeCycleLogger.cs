using UnityEngine;

public class LifeCycleLogger : MonoBehaviour
{
	private void Awake()
	{
		PersistLog.Log($"[AC][F{Time.frameCount}] {name} Awake");
	}

	private void Start()
	{
		PersistLog.Log($"[AC][F{Time.frameCount}] {name} Start");
	}

	private void OnEnable()
	{
		PersistLog.Log($"[AC][F{Time.frameCount}] {name} Enable");
	}

	private void OnDisable()
	{
		PersistLog.Log($"[AC][F{Time.frameCount}] {name} Disable");
	}

	private void OnDestroy()
	{
		PersistLog.Log($"[AC][F{Time.frameCount}] {name} OnDestroy");
	}
}
