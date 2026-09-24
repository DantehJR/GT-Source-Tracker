using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	public Transform[] ChildrenXfs()
	{
		return transform.GetComponentsInChildren<Transform>();
	}
}
