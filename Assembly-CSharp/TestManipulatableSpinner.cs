using UnityEngine;

public class TestManipulatableSpinner : MonoBehaviour
{
	public ManipulatableSpinner spinner;

	public float rotationScale = 1f;

	private void Start()
	{
	}

	private void LateUpdate()
	{
		float angle = spinner.angle;
		transform.rotation = Quaternion.Euler(0f, angle * rotationScale, 0f);
	}
}
