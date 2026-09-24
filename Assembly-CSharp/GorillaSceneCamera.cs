using UnityEngine;

public class GorillaSceneCamera : MonoBehaviour
{
	public GorillaSceneTransform[] sceneTransforms;

	public void SetSceneCamera(int sceneIndex)
	{
		transform.position = sceneTransforms[sceneIndex].scenePosition;
		transform.eulerAngles = sceneTransforms[sceneIndex].sceneRotation;
	}
}
