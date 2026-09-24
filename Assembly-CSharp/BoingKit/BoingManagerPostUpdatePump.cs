using UnityEngine;

namespace BoingKit;

public class BoingManagerPostUpdatePump : MonoBehaviour
{
	private void Start()
	{
		Object.DontDestroyOnLoad(gameObject);
	}

	private bool TryDestroyDuplicate()
	{
		if (BoingManager.s_managerGo == gameObject)
		{
			return false;
		}
		Object.Destroy(gameObject);
		return true;
	}

	private void FixedUpdate()
	{
		if (!TryDestroyDuplicate())
		{
			BoingManager.Execute(BoingManager.UpdateMode.FixedUpdate);
		}
	}

	private void Update()
	{
		if (!TryDestroyDuplicate())
		{
			BoingManager.Execute(BoingManager.UpdateMode.EarlyUpdate);
			BoingManager.PullBehaviorResults(BoingManager.UpdateMode.EarlyUpdate);
			BoingManager.PullReactorResults(BoingManager.UpdateMode.EarlyUpdate);
			BoingManager.PullBonesResults(BoingManager.UpdateMode.EarlyUpdate);
		}
	}

	private void LateUpdate()
	{
		if (!TryDestroyDuplicate())
		{
			BoingManager.PullBehaviorResults(BoingManager.UpdateMode.FixedUpdate);
			BoingManager.PullReactorResults(BoingManager.UpdateMode.FixedUpdate);
			BoingManager.PullBonesResults(BoingManager.UpdateMode.FixedUpdate);
			BoingManager.Execute(BoingManager.UpdateMode.LateUpdate);
			BoingManager.PullBehaviorResults(BoingManager.UpdateMode.LateUpdate);
			BoingManager.PullReactorResults(BoingManager.UpdateMode.LateUpdate);
			BoingManager.PullBonesResults(BoingManager.UpdateMode.LateUpdate);
		}
	}
}
