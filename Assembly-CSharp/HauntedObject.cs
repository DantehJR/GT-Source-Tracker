using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class HauntedObject : MonoBehaviour
{
	private static readonly int _animHaunted = Animator.StringToHash("Haunted");

	private const string _lurkerGhost = "LurkerGhost";

	private const string _wanderingGhost = "WanderingGhost";

	[Tooltip("If this box is checked, then object will rattle when hunted")]
	public bool rattle;

	public float speed = 60f;

	public float amount = 0.01f;

	public float duration = 1f;

	[FormerlySerializedAs("FBX")]
	public GameObject FBXprefab;

	[Tooltip("Use to turn off a game object like candle flames when hunted")]
	public GameObject TurnOffLight;

	public float TurnOffDuration = 2f;

	private Vector3 initialPos;

	private float passedTime;

	private float lightPassedTime;

	private GameObject lurkerGhost;

	private GameObject wanderingGhost;

	private Animator[] animators;

	[SerializeField]
	private AudioSource audioSource;

	[FormerlySerializedAs("rattlingSound")]
	public AudioClip hauntedSound;

	private void Awake()
	{
		this.lurkerGhost = GameObject.FindGameObjectWithTag("LurkerGhost");
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(out var component))
		{
			LurkerGhost lurkerGhost = component;
			lurkerGhost.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(lurkerGhost.TriggerHauntedObjects, new UnityAction<GameObject>(TriggerEffects));
		}
		this.wanderingGhost = GameObject.FindGameObjectWithTag("WanderingGhost");
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(out var component2))
		{
			WanderingGhost wanderingGhost = component2;
			wanderingGhost.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(wanderingGhost.TriggerHauntedObjects, new UnityAction<GameObject>(TriggerEffects));
		}
		animators = transform.GetComponentsInChildren<Animator>();
	}

	private void OnDestroy()
	{
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(out var component))
		{
			LurkerGhost lurkerGhost = component;
			lurkerGhost.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(lurkerGhost.TriggerHauntedObjects, new UnityAction<GameObject>(TriggerEffects));
		}
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(out var component2))
		{
			WanderingGhost wanderingGhost = component2;
			wanderingGhost.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(wanderingGhost.TriggerHauntedObjects, new UnityAction<GameObject>(TriggerEffects));
		}
	}

	private void Start()
	{
		initialPos = transform.position;
		passedTime = 0f;
		lightPassedTime = 0f;
	}

	private void TriggerEffects(GameObject go)
	{
		if (gameObject != go)
		{
			return;
		}
		if (rattle)
		{
			StartCoroutine(Shake());
		}
		if ((bool)audioSource && (bool)hauntedSound)
		{
			audioSource.GTPlayOneShot(hauntedSound);
		}
		if ((bool)FBXprefab)
		{
			ObjectPools.instance.Instantiate(FBXprefab, transform.position);
		}
		if (TurnOffLight != null)
		{
			StartCoroutine(TurnOff());
		}
		Animator[] array = animators;
		foreach (Animator animator in array)
		{
			if ((bool)animator)
			{
				animator.SetTrigger(_animHaunted);
			}
		}
	}

	private IEnumerator Shake()
	{
		while (passedTime < duration)
		{
			passedTime += Time.deltaTime;
			transform.position = new Vector3(initialPos.x + Mathf.Sin(Time.time * speed) * amount, initialPos.y + Mathf.Sin(Time.time * speed) * amount, initialPos.z);
			yield return null;
		}
		passedTime = 0f;
	}

	private IEnumerator TurnOff()
	{
		TurnOffLight.gameObject.SetActive(value: false);
		while (lightPassedTime < TurnOffDuration)
		{
			lightPassedTime += Time.deltaTime;
			yield return null;
		}
		TurnOffLight.SetActive(value: true);
		lightPassedTime = 0f;
	}
}
