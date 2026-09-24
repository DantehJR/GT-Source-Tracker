using GorillaExtensions;
using GorillaLocomotion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class HoverboardVisual : MonoBehaviour, ICallBack
{
	[SerializeField]
	private VRRig parentRig;

	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	[SerializeField]
	[FormerlySerializedAs("audio")]
	private HoverboardAudio hoverboardAudio;

	[SerializeField]
	private HoverboardHandle handlePosition;

	[SerializeField]
	private float grindHapticStrength;

	[SerializeField]
	private float grindHapticDuration;

	[SerializeField]
	private float carveHapticStrength;

	[SerializeField]
	private float carveHapticDuration;

	[SerializeField]
	private MeshRenderer boardMesh;

	[SerializeField]
	private InteractionPoint handleInteractionPoint;

	[SerializeField]
	private TextMeshPro racePositionReadout;

	[SerializeField]
	private TextMeshPro raceLapsReadout;

	private Material colorMaterial;

	private Vector3 interpolatedLocalPosition;

	private Quaternion interpolatedLocalRotation;

	[SerializeField]
	private float lerpIntoHandDuration;

	private float positionLerpSpeed;

	private float rotationLerpSpeed;

	private bool isCallbackActive;

	public Color boardColor { get; private set; }

	public bool IsHeld { get; private set; }

	public bool IsLeftHanded { get; private set; }

	public Vector3 NominalLocalPosition { get; private set; }

	public Quaternion NominalLocalRotation { get; private set; }

	private Transform NominalParentTransform
	{
		get
		{
			if (!IsHeld)
			{
				return transform.parent;
			}
			return (IsLeftHanded ? parentRig.leftHand : parentRig.rightHand).rigTarget.transform;
		}
	}

	private void Awake()
	{
		Material[] sharedMaterials = boardMesh.sharedMaterials;
		colorMaterial = new Material(sharedMaterials[1]);
		sharedMaterials[1] = colorMaterial;
		boardMesh.sharedMaterials = sharedMaterials;
	}

	public void SetIsHeld(bool isHeldLeftHanded, Vector3 localPosition, Quaternion localRotation, Color boardColor)
	{
		if (!isCallbackActive)
		{
			parentRig.AddLateUpdateCallback(this);
			isCallbackActive = true;
		}
		IsHeld = true;
		gameObject.SetActive(value: true);
		IsLeftHanded = isHeldLeftHanded;
		NominalLocalPosition = localPosition;
		NominalLocalRotation = localRotation;
		Transform nominalParentTransform = NominalParentTransform;
		interpolatedLocalPosition = nominalParentTransform.InverseTransformPoint(transform.position);
		interpolatedLocalRotation = nominalParentTransform.InverseTransformRotation(transform.rotation);
		positionLerpSpeed = (interpolatedLocalPosition - NominalLocalPosition).magnitude / lerpIntoHandDuration;
		(Quaternion.Inverse(interpolatedLocalRotation) * NominalLocalRotation).ToAngleAxis(out var angle, out var _);
		rotationLerpSpeed = angle / lerpIntoHandDuration;
		if (parentRig.isLocal)
		{
			GTPlayer.Instance.SetHoverActive(enable: true);
		}
		colorMaterial.color = boardColor;
		this.boardColor = boardColor;
	}

	public void SetNotHeld(bool isLeftHanded)
	{
		IsLeftHanded = isLeftHanded;
		SetNotHeld();
	}

	public void SetNotHeld()
	{
		bool isHeld = IsHeld;
		gameObject.SetActive(value: false);
		IsHeld = false;
		interpolatedLocalPosition = transform.localPosition;
		interpolatedLocalRotation = transform.localRotation;
		positionLerpSpeed = (interpolatedLocalPosition - NominalLocalPosition).magnitude / lerpIntoHandDuration;
		(Quaternion.Inverse(interpolatedLocalRotation) * NominalLocalRotation).ToAngleAxis(out var angle, out var _);
		rotationLerpSpeed = angle / lerpIntoHandDuration;
		if (!isHeld)
		{
			transform.position = transform.parent.TransformPoint(NominalLocalPosition);
			transform.rotation = transform.parent.TransformRotation(NominalLocalRotation);
		}
		if (parentRig.isLocal)
		{
			GTPlayer.Instance.SetHoverActive(enable: false);
		}
		hoverboardAudio.Stop();
	}

	void ICallBack.CallBack()
	{
		Transform nominalParentTransform = NominalParentTransform;
		if ((interpolatedLocalPosition - NominalLocalPosition).IsShorterThan(0.01f))
		{
			transform.position = nominalParentTransform.TransformPoint(NominalLocalPosition);
			transform.rotation = nominalParentTransform.TransformRotation(NominalLocalRotation);
			if (!IsHeld)
			{
				parentRig.RemoveLateUpdateCallback(this);
				isCallbackActive = false;
			}
		}
		else
		{
			interpolatedLocalPosition = Vector3.MoveTowards(interpolatedLocalPosition, NominalLocalPosition, positionLerpSpeed * Time.deltaTime);
			interpolatedLocalRotation = Quaternion.RotateTowards(interpolatedLocalRotation, NominalLocalRotation, rotationLerpSpeed * Time.deltaTime);
			transform.position = nominalParentTransform.TransformPoint(interpolatedLocalPosition);
			transform.rotation = nominalParentTransform.TransformRotation(interpolatedLocalRotation);
		}
		if (IsHeld)
		{
			if (parentRig.isLocal)
			{
				GTPlayer.Instance.SetHoverboardPosRot(transform.position, transform.rotation);
			}
			else
			{
				hoverboardAudio.UpdateAudioLoop(parentRig.LatestVelocity().magnitude, 0f, 0f, 0f);
			}
		}
	}

	public void PlayGrindHaptic()
	{
		if (IsHeld)
		{
			GorillaTagger.Instance.StartVibration(IsLeftHanded, grindHapticStrength, grindHapticDuration);
		}
	}

	public void PlayCarveHaptic(float carveForce)
	{
		if (IsHeld)
		{
			GorillaTagger.Instance.StartVibration(IsLeftHanded, carveForce * carveHapticStrength, carveHapticDuration);
		}
	}

	public void ProxyGrabHandle(bool isLeftHand)
	{
		EquipmentInteractor.instance.UpdateHandEquipment(handlePosition, isLeftHand);
	}

	public void DropFreeBoard()
	{
		FreeHoverboardManager.instance.SendDropBoardRPC(transform.position, transform.rotation, velocityEstimator.linearVelocity, velocityEstimator.angularVelocity, boardColor);
	}

	public void SetRaceDisplay(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			racePositionReadout.gameObject.SetActive(value: false);
			return;
		}
		racePositionReadout.gameObject.SetActive(value: true);
		racePositionReadout.text = text;
	}

	public void SetRaceLapsDisplay(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			raceLapsReadout.gameObject.SetActive(value: false);
			return;
		}
		raceLapsReadout.gameObject.SetActive(value: true);
		raceLapsReadout.text = text;
	}
}
