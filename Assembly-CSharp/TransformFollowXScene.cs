using UnityEngine;

public class TransformFollowXScene : MonoBehaviour
{
	public XSceneRef refToFollow;

	private Transform transformToFollow;

	public Vector3 offset;

	public Vector3 prevPos;

	private void Awake()
	{
		prevPos = transform.position;
	}

	private void Start()
	{
		refToFollow.TryResolve(out transformToFollow);
	}

	private void LateUpdate()
	{
		prevPos = transform.position;
		transform.rotation = transformToFollow.rotation;
		transform.position = transformToFollow.position + transformToFollow.rotation * offset;
	}
}
