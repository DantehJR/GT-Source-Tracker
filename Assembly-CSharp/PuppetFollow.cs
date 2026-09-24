using UnityEngine;

public class PuppetFollow : MonoBehaviour
{
	public Transform sourceTarget;

	public Transform sourceBase;

	public Transform puppetBase;

	private void FixedUpdate()
	{
		transform.position = sourceTarget.position - sourceBase.position + puppetBase.position;
		transform.localRotation = sourceTarget.localRotation;
	}
}
