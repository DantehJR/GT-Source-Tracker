using UnityEngine;

namespace GorillaTag;

public class DestroyOnAwake : MonoBehaviour
{
	protected void Awake()
	{
		try
		{
			Object.Destroy(gameObject);
		}
		catch
		{
		}
	}

	protected void OnEnable()
	{
		try
		{
			Object.Destroy(gameObject);
		}
		catch
		{
		}
	}

	protected void Update()
	{
		try
		{
			Object.Destroy(gameObject);
		}
		catch
		{
		}
	}
}
