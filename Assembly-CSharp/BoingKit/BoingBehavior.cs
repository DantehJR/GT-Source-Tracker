using UnityEngine;

namespace BoingKit;

public class BoingBehavior : BoingBase
{
	public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

	public bool TwoDDistanceCheck;

	public bool TwoDPositionInfluence;

	public bool TwoDRotationInfluence;

	public bool EnablePositionEffect = true;

	public bool EnableRotationEffect = true;

	public bool EnableScaleEffect;

	public bool GlobalReactionUpVector;

	public BoingManager.TranslationLockSpace TranslationLockSpace;

	public bool LockTranslationX;

	public bool LockTranslationY;

	public bool LockTranslationZ;

	public BoingWork.Params Params;

	public SharedBoingParams SharedParams;

	internal bool PositionSpringDirty;

	internal bool RotationSpringDirty;

	internal bool ScaleSpringDirty;

	internal bool CachedTransformValid;

	internal Vector3 CachedPositionLs;

	internal Vector3 CachedPositionWs;

	internal Vector3 RenderPositionWs;

	internal Quaternion CachedRotationLs;

	internal Quaternion CachedRotationWs;

	internal Quaternion RenderRotationWs;

	internal Vector3 CachedScaleLs;

	internal Vector3 RenderScaleLs;

	internal bool InitRebooted;

	public Vector3Spring PositionSpring
	{
		get
		{
			return Params.Instance.PositionSpring;
		}
		set
		{
			Params.Instance.PositionSpring = value;
			PositionSpringDirty = true;
		}
	}

	public QuaternionSpring RotationSpring
	{
		get
		{
			return Params.Instance.RotationSpring;
		}
		set
		{
			Params.Instance.RotationSpring = value;
			RotationSpringDirty = true;
		}
	}

	public Vector3Spring ScaleSpring
	{
		get
		{
			return Params.Instance.ScaleSpring;
		}
		set
		{
			Params.Instance.ScaleSpring = value;
			ScaleSpringDirty = true;
		}
	}

	public BoingBehavior()
	{
		Params.Init();
	}

	public virtual void Reboot()
	{
		Params.Instance.PositionSpring.Reset(transform.position);
		Params.Instance.RotationSpring.Reset(transform.rotation);
		Params.Instance.ScaleSpring.Reset(transform.localScale);
		CachedPositionLs = transform.localPosition;
		CachedRotationLs = transform.localRotation;
		CachedPositionWs = transform.position;
		CachedRotationWs = transform.rotation;
		CachedScaleLs = transform.localScale;
		CachedTransformValid = true;
	}

	public virtual void OnEnable()
	{
		CachedTransformValid = false;
		InitRebooted = false;
		Register();
	}

	public void Start()
	{
		InitRebooted = false;
	}

	public virtual void OnDisable()
	{
		Unregister();
	}

	protected virtual void Register()
	{
		BoingManager.Register(this);
	}

	protected virtual void Unregister()
	{
		BoingManager.Unregister(this);
	}

	public void UpdateFlags()
	{
		Params.Bits.SetBit(0, TwoDDistanceCheck);
		Params.Bits.SetBit(1, TwoDPositionInfluence);
		Params.Bits.SetBit(2, TwoDRotationInfluence);
		Params.Bits.SetBit(3, EnablePositionEffect);
		Params.Bits.SetBit(4, EnableRotationEffect);
		Params.Bits.SetBit(5, EnableScaleEffect);
		Params.Bits.SetBit(6, GlobalReactionUpVector);
		Params.Bits.SetBit(9, UpdateMode == BoingManager.UpdateMode.FixedUpdate);
		Params.Bits.SetBit(10, UpdateMode == BoingManager.UpdateMode.EarlyUpdate);
		Params.Bits.SetBit(11, UpdateMode == BoingManager.UpdateMode.LateUpdate);
	}

	public virtual void PrepareExecute()
	{
		PrepareExecute(accumulateEffectors: false);
	}

	protected void PrepareExecute(bool accumulateEffectors)
	{
		if (SharedParams != null)
		{
			BoingWork.Params.Copy(ref SharedParams.Params, ref Params);
		}
		UpdateFlags();
		Params.InstanceID = GetInstanceID();
		Params.Instance.PrepareExecute(ref Params, CachedPositionWs, CachedRotationWs, transform.localScale, accumulateEffectors);
	}

	public void Execute(float dt)
	{
		Params.Execute(dt);
	}

	public void PullResults()
	{
		PullResults(ref Params);
	}

	public void GatherOutput(ref BoingWork.Output o)
	{
		if (BoingManager.UseAsynchronousJobs)
		{
			if (PositionSpringDirty)
			{
				PositionSpringDirty = false;
			}
			else
			{
				Params.Instance.PositionSpring = o.PositionSpring;
			}
			if (RotationSpringDirty)
			{
				RotationSpringDirty = false;
			}
			else
			{
				Params.Instance.RotationSpring = o.RotationSpring;
			}
			if (ScaleSpringDirty)
			{
				ScaleSpringDirty = false;
			}
			else
			{
				Params.Instance.ScaleSpring = o.ScaleSpring;
			}
		}
		else
		{
			Params.Instance.PositionSpring = o.PositionSpring;
			Params.Instance.RotationSpring = o.RotationSpring;
			Params.Instance.ScaleSpring = o.ScaleSpring;
		}
	}

	private void PullResults(ref BoingWork.Params p)
	{
		CachedPositionLs = transform.localPosition;
		CachedPositionWs = transform.position;
		RenderPositionWs = BoingWork.ComputeTranslationalResults(transform, transform.position, p.Instance.PositionSpring.Value, this);
		transform.position = RenderPositionWs;
		CachedRotationLs = transform.localRotation;
		CachedRotationWs = transform.rotation;
		RenderRotationWs = p.Instance.RotationSpring.ValueQuat;
		transform.rotation = RenderRotationWs;
		CachedScaleLs = transform.localScale;
		RenderScaleLs = p.Instance.ScaleSpring.Value;
		transform.localScale = RenderScaleLs;
		CachedTransformValid = true;
	}

	public virtual void Restore()
	{
		if (!CachedTransformValid)
		{
			return;
		}
		if (Application.isEditor)
		{
			if ((transform.position - RenderPositionWs).sqrMagnitude < 0.0001f)
			{
				transform.localPosition = CachedPositionLs;
			}
			if (QuaternionUtil.GetAngle(transform.rotation * Quaternion.Inverse(RenderRotationWs)) < 0.01f)
			{
				transform.localRotation = CachedRotationLs;
			}
			if ((transform.localScale - RenderScaleLs).sqrMagnitude < 0.0001f)
			{
				transform.localScale = CachedScaleLs;
			}
		}
		else
		{
			transform.localPosition = CachedPositionLs;
			transform.localRotation = CachedRotationLs;
			transform.localScale = CachedScaleLs;
		}
	}
}
