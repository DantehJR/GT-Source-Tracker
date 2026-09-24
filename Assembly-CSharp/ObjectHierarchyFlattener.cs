using GorillaTag;
using UnityEngine;

[DefaultExecutionOrder(2001)]
public class ObjectHierarchyFlattener : MonoBehaviour, IGorillaSimpleBackgroundWorker
{
	public const int k_monoDefaultExecutionOrder = 2001;

	[DebugReadout]
	private GameObject originalParentGO;

	private Transform originalParentTransform;

	private Vector3 originalLocalPosition;

	private Vector3 calcOffset;

	private Quaternion originalLocalRotation;

	private Vector3 originalScale;

	private float originalParentScale;

	public bool trackTransformOfParent;

	public bool maintainRelativeScale;

	private FlattenerCrumb crumb;

	public Transform overrideParentTransform;

	private bool isAttachedToOverride;

	private bool initialized;

	private bool abandonWork = true;

	private void ResetTransform()
	{
		if (initialized && (!(originalParentGO != null) || !originalParentGO.activeInHierarchy))
		{
			transform.SetParent(originalParentTransform);
			isAttachedToOverride = false;
			transform.localPosition = originalLocalPosition;
			transform.localRotation = originalLocalRotation;
			transform.localScale = originalScale;
			initialized = false;
		}
	}

	public void CrumbDisabled()
	{
		if (!ApplicationQuittingState.IsQuitting)
		{
			if (trackTransformOfParent)
			{
				ObjectHierarchyFlattenerManager.UnregisterOHF(this);
			}
			if (this != null)
			{
				Invoke("ResetTransform", 0f);
			}
		}
	}

	public void InvokeLateUpdate()
	{
		if (maintainRelativeScale)
		{
			transform.localScale = Vector3.Scale(originalParentTransform.lossyScale, originalScale);
		}
		transform.rotation = originalParentTransform.rotation * originalLocalRotation;
		transform.position = originalParentTransform.position + transform.rotation * calcOffset * (originalParentTransform.lossyScale.x / originalParentScale) * originalParentScale;
	}

	private void OnEnable()
	{
		abandonWork = false;
		GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
	}

	private void OnDisable()
	{
		abandonWork = true;
		ObjectHierarchyFlattenerManager.UnregisterOHF(this);
		if (enabled)
		{
			Invoke("ResetTransformIfStillDisabled", 0f);
		}
	}

	private void OnDestroy()
	{
		CancelInvoke();
	}

	private void ResetTransformIfStillDisabled()
	{
		if (!isActiveAndEnabled)
		{
			ResetTransform();
		}
	}

	public void SimpleWork()
	{
		if (initialized || abandonWork)
		{
			return;
		}
		if (trackTransformOfParent)
		{
			ObjectHierarchyFlattenerManager.RegisterOHF(this);
		}
		if (!isAttachedToOverride)
		{
			originalParentTransform = transform.parent;
			originalParentGO = originalParentTransform.gameObject;
			originalLocalPosition = transform.localPosition;
			originalLocalRotation = transform.localRotation;
			originalParentScale = transform.parent.lossyScale.x;
			originalScale = transform.localScale;
			calcOffset = Vector3.Scale(originalLocalPosition, originalScale);
			FlattenerCrumb flattenerCrumb = originalParentGO.GetComponent<FlattenerCrumb>();
			if (flattenerCrumb == null)
			{
				flattenerCrumb = originalParentGO.AddComponent<FlattenerCrumb>();
			}
			flattenerCrumb.AddFlattenerReference(this);
		}
		transform.SetParent((overrideParentTransform != null) ? overrideParentTransform : null);
		isAttachedToOverride = true;
		initialized = true;
	}
}
