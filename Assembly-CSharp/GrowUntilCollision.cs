using UnityEngine;

public class GrowUntilCollision : MonoBehaviour
{
	[SerializeField]
	private float maxSize = 10f;

	[SerializeField]
	private float initialRadius = 1f;

	[SerializeField]
	private float minRetriggerTime = 1f;

	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	private AudioSource audioSource;

	private float maxVolume;

	private float maxPitch;

	private float timeSinceTrigger;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource != null)
		{
			maxVolume = audioSource.volume;
			maxPitch = audioSource.pitch;
		}
		zero();
	}

	private void zero()
	{
		transform.localScale = Vector3.one * initialRadius;
		if (audioSource != null)
		{
			audioSource.volume = 0f;
			audioSource.pitch = 1f;
		}
		timeSinceTrigger = 0f;
	}

	private void OnTriggerEnter(Collider other)
	{
		tryToTrigger(transform.position, other.transform.position);
	}

	private void OnTriggerExit(Collider other)
	{
		tryToTrigger(transform.position, other.transform.position);
	}

	private void OnCollisionEnter(Collision collision)
	{
		tryToTrigger(transform.position, collision.GetContact(0).point);
	}

	private void OnCollisionExit(Collision collision)
	{
		tryToTrigger(transform.position, collision.GetContact(0).point);
	}

	private void tryToTrigger(Vector3 p1, Vector3 p2)
	{
		if (timeSinceTrigger > minRetriggerTime)
		{
			if (colliderFound != null)
			{
				colliderFound.Invoke(p1, p2);
			}
			zero();
		}
	}

	private void Update()
	{
		float num = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
		if (transform.localScale.x < maxSize * num)
		{
			transform.localScale += Vector3.one * Time.deltaTime * num;
			if (audioSource != null)
			{
				audioSource.volume = maxVolume * (transform.localScale.x / maxSize);
				audioSource.pitch = 1f + maxPitch * (transform.localScale.x / maxSize);
			}
		}
		timeSinceTrigger += Time.deltaTime;
	}
}
