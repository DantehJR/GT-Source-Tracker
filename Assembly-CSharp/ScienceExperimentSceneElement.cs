using GorillaTag;
using UnityEngine;

public class ScienceExperimentSceneElement : MonoBehaviour, ITickSystemPost
{
	public ScienceExperimentElementID elementID;

	private Transform followElement;

	bool ITickSystemPost.PostTickRunning { get; set; }

	void ITickSystemPost.PostTick()
	{
		transform.position = followElement.position;
		transform.rotation = followElement.rotation;
		transform.localScale = followElement.localScale;
	}

	private void Start()
	{
		followElement = ScienceExperimentManager.instance.GetElement(elementID);
		TickSystem<object>.AddPostTickCallback(this);
	}

	private void OnDestroy()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}
}
