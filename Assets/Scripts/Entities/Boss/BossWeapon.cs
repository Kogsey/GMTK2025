using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class BossWeapon : MonoBehaviour
{
	public bool CanDoNewAttack { get; private set; } = true;
	public bool IsThrown { get; private set; }
	public FrameData[] Swing1;
	public FrameData[] Swing2;
	public Sprite IdleFrame;
	public Sprite ThrowFrameS;

	public int ThrowFrame;
	public SpriteRenderer Renderer;
	public Collider2D Collider;
	public BossController BossController;

	private AnimationHelper animationHelper;
	private const int FlipMod = -1;
	protected int FaceDirection { get => (int)transform.localScale.x * FlipMod; set => transform.localScale = new Vector3(value * FlipMod, 1, 1); }

	private const int deltaCounts = 5;
	private float smoothDeltaX;
	private Vector2 Centre
	{
		/*		get => Renderer.bounds.center;
				set
				{
					Bounds bounds = Renderer.bounds;
					bounds.center = value;
					Renderer.bounds = bounds;
				}*/

		get => transform.position;
		set
		{
			smoothDeltaX -= smoothDeltaX / deltaCounts;
			smoothDeltaX += (value.x - Centre.x) / deltaCounts;
			transform.position = value;
		}
	}

	private float Rotation
	{
		get => transform.rotation.eulerAngles.z;
		set => transform.rotation = Quaternion.Euler(0, 0, value);
	}

	private void Awake()
	{
		if (Renderer == null)
			Renderer = GetComponent<SpriteRenderer>();
		if (Collider == null)
			Collider = GetComponent<Collider2D>();
		if (BossController == null)
			BossController = GetComponent<BossController>();

		animationHelper = new(Renderer);
	}

	public Vector2 HoldOffset;
	private Vector2 HomePos => BossController.Centre + HoldOffset;

	public float thrownSpeed;
	private Vector2 throwTarget;
	private Vector2 _internalTarget;
	private bool onReturn;
	public float DistanceCheck = 0.5f;

	public void Throw(Vector2 position)
	{
		IsThrown = true;
		onReturn = false;
		throwTarget = position;
		_internalTarget = HomePos;
		CanDoNewAttack = false;
	}

	private float _internalRotation = 0f;
	public float RotationSpeed;
	public float InterpolationStrength = 15f;
	private void UpdateRotation()
	{
		if (IsThrown)
		{
			_internalRotation += RotationSpeed * Time.deltaTime;
			_internalRotation %= 360;
		}
		else
		{
			_internalRotation = 0f;
		}
		Rotation = Extensions.SmoothInterpolateAngle(Rotation, _internalRotation, InterpolationStrength);
	}

	public void Update()
	{
		if (IsThrown)
		{
			Vector2 target = onReturn ? HomePos : throwTarget;
			Vector2 throwDirection = (target - _internalTarget).normalized;

			_internalTarget += Time.deltaTime * thrownSpeed * throwDirection;
			Centre = Extensions.SmoothInterpolate(Centre, _internalTarget, InterpolationStrength);

			if (Vector2.Distance(_internalTarget, throwTarget) < DistanceCheck)
				onReturn = true;

			if (onReturn && Vector2.Distance(_internalTarget, HomePos) < DistanceCheck)
			{
				CanDoNewAttack = true;
				IsThrown = false;
			}
		}
		else
		{
			Centre = Extensions.SmoothInterpolate(Centre, HomePos, InterpolationStrength);
		}

		if (IsThrown)
		{
			FaceDirection = BossController.FaceDirection;

		}
		else
		{
			if (smoothDeltaX > 0)
				FaceDirection = 1;
			else if (smoothDeltaX < 0)
				FaceDirection = -1;
		}

		UpdateRotation();
	}
}