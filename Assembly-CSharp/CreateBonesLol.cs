using UnityEngine;

public class CreateBonesLol : MonoBehaviour
{
	public GameObject cube;

	public OVRSkeleton skeleton;

	private void Update()
	{
		if (skeleton.Bones.Count <= 0)
		{
			return;
		}
		foreach (OVRBone bone in skeleton.Bones)
		{
			GameObject gameObject = Object.Instantiate(cube);
			gameObject.transform.parent = bone.Transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
		}
		enabled = false;
	}
}
