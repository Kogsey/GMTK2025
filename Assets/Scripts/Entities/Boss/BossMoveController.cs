// Ignore Spelling: Mult HitPoints Collider

using System;
using UnityEngine;
using Utilities.Movement;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GroundCheck))]
public class BossController : EnemyBehave
{
	public float CutSceneTimer = 3f;

	[Header("Movement")]
	public float PreJumpTimerMax = 0.4f;

	public float JumpDelayTimerMax = 0.5f;
	public float ExtendedJumpTimerMax = 0.4f;
	public float FloatTimerMax = 3f;

	[Space]
	public float BaseSpeedForce;

	public float BaseJumpImpulse;

	[Space]
	public float FloatSpeedCap;

	public float GroundSpeedCap;

	[Space]
	public float HighJumpMinSpeed;

	public float HighJumpForceMult;
	public float HighJumpTimeMult;

	[Space]
	public float DodgeTime = 0.2f;

	public float DodgeSpeedOverride;

	public bool InDodgeRails;
	public float DodgeNoImmuneTime;
	private bool Cutscene { get; set; }
	private bool IsImmune => InDodgeRails || Cutscene;

	[Header("Environment")]
	public float GravityScale;

	public float ExtendedJumpGravityScale;

	[Range(0f, 1f)]
	public float XDrag;

	[Range(0f, 1f)]
	public float XDragWhenNotMoving;

	[Range(0f, 1f)]
	public float YDrag;

	[Header("Animation")]
	public float MinimumAnimationSpeed;

	public FrameData[] WalkFrames;

	public FrameData[] DeathFrames;
	public FrameData[] JumpFrames;
	public FrameData[] IdleFrames;
	public Sprite Dodge;

	private bool CanJump => GroundCheck.GetGrounded();
	private bool OnGround => GroundCheck.GetGroundedPure();
	private float FallSpeedCap => 10 * FloatSpeedCap;
	private float AirSpeedCap => 1.5f * GroundSpeedCap;

	public BossProjectile RangedAttackPrefab;

	private GroundCheck GroundCheck { get; set; }
	private AnimationHelper AnimationHelper { get; set; }
	private EntityHealth HealthData { get; set; }
	public BossWeapon BossWeapon;

	private PhysicsVelocity2D Velocity;

	protected override void InternalAwake()
	{
		base.InternalAwake();
		GroundCheck = GetComponent<GroundCheck>();
		HealthData = GetComponent<EntityHealth>();
		AnimationHelper = new(SpriteRenderer);
	}

	// Start is called before the first frame update
	protected override void InternalStart()
	{
		base.InternalStart();

		RigidBody.gravityScale = GravityScale;
		AnimationHelper.ForceToLoop(IdleFrames);
	}

	#region Attacks

	public override bool HasTarget => true;
	public override Vector2 TargetPos => Player.BoundsCheckingRect.center;
	public override bool AutoFlip => false;
	protected override int FlipMod => -1;
	public bool PhaseTwo { get; set; }

	private int MovementDirection { get; set; }

	private float AttackTimer { get; set; }
	private float MaxAttackTimer { get; set; } = 10f;
	private Action CurrentAttack { get; set; }
	public Vector2 Centre => BoundsCheckingRect.center;

	private int GetDirectionTo(float position)
		=> Centre.x < position ? 1 : -1;

	private int _dodgeDirection;
	public float MinPlayerDistanceForDodge;
	public float MinPlayerDistanceForThrow;
	private bool _dodgeFollowUp = false;

	private const int attackMax = 5;

	private void UpdateAttack()
	{
		if (AttackTimer >= MaxAttackTimer)
			EndAttack();

		CurrentAttack ??= RollAttack();
		CurrentAttack.Invoke();
		if (HealthData != null)
			HealthData.OtherImmune = IsImmune;
	}

	private Action RollAttack()
	{
		int moveNumber = Random.Range(0, attackMax);

		if (_dodgeFollowUp)
		{
			_dodgeFollowUp = false;
			moveNumber = Random.Range(4, attackMax);
		}

		switch (moveNumber)
		{
			case 1:
			case 0:
				float targetPos = TargetPos.x;
				int direction = GetDirectionTo(targetPos);
				MaxAttackTimer = 2f;
				return () => RunToPoint(targetPos, direction);

			case 2:

				float holdFor = Random.Range(1, 3);
				MaxAttackTimer = holdFor * 1.5f;
				willThrowCheck = /*Random.value < 0.5f*/true;

				return () => DynamicJump(holdFor);

			case 3:
				if (TargetVector.magnitude > MinPlayerDistanceForDodge)
					goto case 0;

				_dodgeDirection = -GetDirectionTo(TargetPos.x);
				MaxAttackTimer = DodgeTime;
				InDodgeRails = true;
				return DodgeMovement;

			case 4:
				MaxAttackTimer = 3f;
				return RangedAttack;

			default:
				MaxAttackTimer = 2f;
				return ChasePlayer;
		}
	}

	public Vector2 RangedAttackOffset;
	public float RangedAttackPrePause = 0.2f;
	public float RangedAttackPostPause = 0.1f;
	public float RangedAttackShootSpeed = 4f;
	private bool _hasRangedAttacked = false;

	private void RangedAttack()
	{
		FaceDirection = GetDirectionTo(TargetPos.x);

		if (!_hasRangedAttacked)
		{
			if (AttackTimer >= RangedAttackPrePause)
			{
				Vector2 spawnPos = Centre + RangedAttackOffset;
				BossProjectile bossProjectile = Instantiate(RangedAttackPrefab, spawnPos, Quaternion.identity);
				bossProjectile.SetShoot(RangedAttackShootSpeed * FaceDirection * Vector2.right);
				_hasRangedAttacked = true;
			}
		}
		else if (AttackTimer >= RangedAttackPostPause + RangedAttackPrePause)
		{
			EndAttack();
		}
	}

	private void EndAttack()
	{
		_hasRangedAttacked = false;
		AttackTimer = 0f;
		CurrentAttack = null;
		hasJumped = false;
		InDodgeRails = false;
	}

	private void ChasePlayer()
	{
		MovementDirection = GetDirectionTo(TargetPos.x);
		MoveInDirection(MovementDirection);
	}

	private void RunToPoint(float xPos, int direction)
	{
		MovementDirection = direction;
		int trueDir = GetDirectionTo(xPos);
		if (trueDir != direction)
			EndAttack();
		else
			MoveInDirection(direction);
	}

	bool hasJumped = false;
	bool willThrowCheck;

	private void DynamicJump(float heldTime)
	{
		ChasePlayer();
		JumpMovement(AttackTimer < heldTime);

		if (JumpState != Jump.None)
			hasJumped = true;

		if (hasJumped && Velocity.y < 0)
		{
			if (willThrowCheck && BossWeapon.CanDoNewAttack && TargetVector.magnitude > MinPlayerDistanceForThrow)
			{
				willThrowCheck = false;
				BossWeapon.Throw(TargetPos);
			}
		}

		if (hasJumped && JumpState == Jump.None)
			EndAttack();
	}

	public void DodgeMovement()
	{
		_dodgeFollowUp = true;
		FaceDirection = -_dodgeDirection;
		Velocity.SetVelocity(DodgeSpeedOverride * _dodgeDirection * Vector2.right);
		InDodgeRails = true;
	}

	#endregion Attacks

	#region Movement

	// Update is called once per frame
	private void FixedUpdate()
	{
		Velocity = new PhysicsVelocity2D(RigidBody.velocity);
		UpdateTimers();

		if (!Dead && CutSceneTimer < 0)
			UpdateAttack();

		FinalPhysicsUpdate();
		ChooseFrame();
	}

	public void UpdateTimers()
	{
		JumpStateTimer -= Velocity.DeltaTime;
		AttackTimer += Velocity.DeltaTime;
		CutSceneTimer -= Velocity.DeltaTime;
	}

	public void MoveInDirection(int direction)
	{
		FaceDirection = direction;
		Velocity += BaseSpeedForce * FaceDirection * Vector2.right;
	}

	private enum Jump
	{
		None, // Moves to PreJump or Floating
		PreJump, // Moves to HighJump
		HighJump, // Moves to JumpDelay

		//Floating, // Moves to none
		JumpDelay, //Moves to none
	}

	private Jump _jumpState = Jump.None;

	private Jump JumpState
	{
		get => _jumpState;
		set
		{
			switch (value)
			{
				case Jump.None:
					JumpStateTimer = 0;
					break;

				case Jump.PreJump:
					JumpStateTimer = PreJumpTimerMax;
					break;

				case Jump.HighJump:
					JumpStateTimer = ExtendedJumpTimerMax * (Mathf.Abs(Velocity.x) > HighJumpMinSpeed ? HighJumpTimeMult : 1f);
					break;

				case Jump.JumpDelay:
					JumpStateTimer = JumpDelayTimerMax;
					break;
			}
			_jumpState = value;
		}
	}

	private float JumpStateTimer = -1;

	public void JumpMovement(bool holdingJump)
	{
		switch (JumpState)
		{
			case Jump.None:
				if (CanJump && holdingJump)// If press jump key and can jump
				{
					JumpState = Jump.PreJump; // Start jump animation
											  //FloatTimeLeft = FloatTimerMax;
				}
				/*				else if (FloatTimeLeft > 0 && jumpKey && !OnGround) // else if can float and jump key down
								{
									JumpState = Jump.Floating;
								}*/

				break;

			case Jump.PreJump:
				if (JumpStateTimer <= 0)
				{
					if (Velocity.y < 0)
						Velocity.y = 0;
					Velocity += (Mathf.Abs(Velocity.x) > HighJumpMinSpeed ? HighJumpForceMult : 1f) * BaseJumpImpulse * Vector2.up; // Boost internalVelocity
					JumpState = Jump.HighJump;
				}
				break;

			case Jump.HighJump:
				RigidBody.gravityScale = 0.1f;
				if (JumpStateTimer <= 0 || !holdingJump || OnGround)
				{
					JumpState = Jump.JumpDelay;
					RigidBody.gravityScale = GravityScale;
				}
				break;

			case Jump.JumpDelay:
				if (JumpStateTimer < 0)
					JumpState = Jump.None;
				break;
		}
	}

	#endregion Movement

	#region Physics

	public void FinalPhysicsUpdate()
	{
		Velocity.x *= GetXDrag();
		Velocity.y *= YDrag;

		CapSpeed();
		if (OnGround)
			Velocity.OnGround();
		//Velocity.StepThenApplyTo(transform);
		RigidBody.velocity = Velocity.GetTotalVelocity();
	}

	public float GetXDrag()
		=> (MovementDirection != 0) // If pressing movement key
			? (((Velocity.x < 0 && MovementDirection > 0) || (Velocity.x > 0 && MovementDirection < 0)) // and if pressing the opposite direction to movement
				? XDragWhenNotMoving // High drag when moving in opposite direction to velocity
				: XDrag) // Low drag when moving in same dir as velocity
				: XDragWhenNotMoving; // High drag when pressing nothing

	public void CapSpeed()
	{
		if (!CanJump)
		{
			CapFall(/*JumpState == Jump.Floating ? FloatSpeedCap :*/ FallSpeedCap);
			CapMovement(AirSpeedCap);
		}
		CapMovement(GroundSpeedCap);
	}

	public void CapFall(float speed)
	{
		if (Velocity.y < speed)
			Velocity.y = speed;
	}

	public void CapMovement(float speed)
	{
		if (Velocity.x < -speed)
			Velocity.x = -speed;
		else if (Velocity.x > speed)
			Velocity.x = speed;
	}

	#endregion Physics

	#region Sprites

	private bool RunningSpeedMet => Math.Abs(Velocity.x) > MinimumAnimationSpeed;
	private bool RiseSpeedMet => Velocity.y > MinimumAnimationSpeed;

	private void ChooseFrame()
	{
		if (InDodgeRails)
		{
			AnimationHelper.CheckedSwapToFrame(Dodge);
		}
		else if (JumpState == Jump.PreJump)
		{
			AnimationHelper.CheckedSwapToFrame(JumpFrames[2].Frame);
		}
		else if (RiseSpeedMet)
		{
			AnimationHelper.CheckedSwapToFrame(JumpFrames[0].Frame);
		}
		else if (!OnGround)
		{
			AnimationHelper.CheckedSwapToFrame(JumpFrames[1].Frame);
		}
		else if (OnGround)
		{
			if (RunningSpeedMet && JumpState == Jump.None)
				AnimationHelper.CheckedSwapToSequence(WalkFrames);
			else
				AnimationHelper.CheckedSwapToSequence(IdleFrames);
		}

		AnimationHelper.Update();

		/*		if (JumpState == Jump.PreJump)
				{
					SetCurrentSprite(JumpFrames[0]);
				}
				else if (!CanJump)
				{
					if (Velocity.y > 0)
						SetCurrentSprite(JumpFrames[1]);
					else if (JumpState == Jump.Floating)
						SetCurrentSprite(FloatFrame);
					else
						SetCurrentSprite(FallFrame);
				}
				else
				{
					SetCurrentSprite(walkFrame);
				}*/
	}

	#endregion Sprites

	#region Death

	public override void OnDeath()
	{
		Dead = true;
		AnimationHelper.CheckedSwapToSequence(DeathFrames);
		AnimationHelper.LoopAnimation = false;
		AnimationHelper.FreezeChanges = true;
		RigidBody.gravityScale = GravityScale;

		if (TryGetComponent(out Aggressor aggressor))
			aggressor.HitInfo = HitInfo.GetImpotent();

		if (BossWeapon.TryGetComponent(out aggressor))
			aggressor.HitInfo = HitInfo.GetImpotent();
	}

	protected override float DeathTimerMax => 3f;

	protected override void OnDeathTimerEnd()
		=> BetterSingleton<GameplayLoop>.Instance.OnBossBeaten();

	#endregion Death

	#region Editor

	private void OnDrawGizmosSelected()
	{
		if (SpriteRenderer == null)
			SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		Extensions.GizmosDrawRect(BoundsCheckingRect);
	}

	#endregion Editor
}