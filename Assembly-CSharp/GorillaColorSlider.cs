using UnityEngine;

public class GorillaColorSlider : MonoBehaviour
{
	public bool setRandomly;

	public float zRange;

	public float maxValue;

	public float minValue;

	public Vector3 startingLocation;

	public int valueIndex;

	public float valueImReporting;

	public GorillaTriggerBox gorilla;

	private float startingZ;

	private void Start()
	{
		if (!setRandomly)
		{
			startingLocation = transform.position;
		}
	}

	public void SetPosition(float speed)
	{
		float num = startingLocation.x - zRange / 2f;
		float num2 = startingLocation.x + zRange / 2f;
		float x = (speed - minValue) * (num2 - num) / (maxValue - minValue) + num;
		transform.position = new Vector3(x, startingLocation.y, startingLocation.z);
		valueImReporting = InterpolateValue(transform.position.x);
	}

	public float InterpolateValue(float value)
	{
		float num = startingLocation.x - zRange / 2f;
		float num2 = startingLocation.x + zRange / 2f;
		return (value - num) / (num2 - num) * (maxValue - minValue) + minValue;
	}

	public void OnSliderRelease()
	{
		if (zRange != 0f && (transform.position - startingLocation).magnitude > zRange / 2f)
		{
			if (transform.position.x > startingLocation.x)
			{
				transform.position = new Vector3(startingLocation.x + zRange / 2f, startingLocation.y, startingLocation.z);
			}
			else
			{
				transform.position = new Vector3(startingLocation.x - zRange / 2f, startingLocation.y, startingLocation.z);
			}
		}
		valueImReporting = InterpolateValue(transform.position.x);
	}
}
