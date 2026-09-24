using UnityEngine;

public class Positionable : MonoBehaviour
{
	public void CopyPostion(Transform t)
	{
		transform.position = t.position;
	}

	public void StickRightUnder(Transform t)
	{
		transform.position = t.position;
		transform.localPosition = Vector3.zero;
	}
}
