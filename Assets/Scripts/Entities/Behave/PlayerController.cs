// Ignore Spelling: Mult HitPoints Collider

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utilities.Movement;
using Utilities.Timing;

[RequireComponent(typeof(GroundCheck))]
public class PlayerController : EntityBehave, ISingleton
{
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
	public float DodgeTime = 0.5f;

	public float DodgeSpeedOverride;

	public bool InDodgeRails;
	public float DodgeNoImmuneTime;
	private bool IsDodgeImmune => InDodgeRails && DodgeStateTimer <= (DodgeTime - DodgeNoImmuneTime);

	public int DodgesLeft;
	public int maxDodges = 1;

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

	private AnimationHelper animationHelper;

	private bool CanJump => groundCheck.GetGrounded();
	private bool OnGround => groundCheck.GetGroundedPure();
	private float FallSpeedCap => 10 * FloatSpeedCap;
	private float AirSpeedCap => 1.5f * GroundSpeedCap;

	private EntityHealth healthData;
	private GroundCheck groundCheck;
	private SwordController sword;
	private PhysicsVelocity2D Velocity;
	private float DodgeStateTimer;

	public List<ItemDrop> Items = new();

	public void ResetStats()
	{
	}

	public void RegenerateStats()
	{
	}

	#region Movement

	protected override void InternalAwake()
	{
		base.InternalAwake();

		healthData = GetComponent<EntityHealth>();
		groundCheck = GetComponent<GroundCheck>();
		sword = GetComponentInChildren<SwordController>();
		animationHelper = new(SpriteRenderer);
	}

	// Start is called before the first frame update
	protected override void InternalStart()
	{
		base.InternalStart();

		RigidBody.gravityScale = GravityScale;

		animationHelper.ForceToLoop(IdleFrames);
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		Velocity = new PhysicsVelocity2D(RigidBody.velocity);
		UpdateTimers();

		Movement();

		FinalPhysicsUpdate();
		ChooseFrame();
	}

	void Update()
	{
		if (!Dead)
		{
			if (Input.GetKeyDown(Settings.CurrentSettings.Jump))
				QueueJumpTimer.Reset();
			if (Input.GetKeyDown(Settings.CurrentSettings.Dodge))
				QueueDodgeTimer.Reset();
		}
		else
		{
			DeathTime -= Time.deltaTime;
			if (DeathTime <= 0)
				BetterSingleton<GameplayLoop>.Instance.OnDeath();
		}
	}

	public void UpdateTimers()
	{
		DodgeStateTimer -= Velocity.DeltaTime; // Important to use internalVelocity's delta time
											   //FloatTimeLeft -= Velocity.DeltaTime;
		JumpStateTimer -= Velocity.DeltaTime;

		QueueJumpTimer.Tick();
		QueueDodgeTimer.Tick();
	}

	public void Movement()
	{
		if (!Dead)
		{
			DodgeMovement();

			if (!InDodgeRails)
			{
				if (Input.GetKey(Settings.CurrentSettings.Left) ^ Input.GetKey(Settings.CurrentSettings.Right)) //Exclusive or so do nothing if both held
					MoveInDirection(Input.GetKey(Settings.CurrentSettings.Left) ? -1 : 1); // Directional movement
				JumpMovement();
			}
		}
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

	private Jump _playerJumpState = Jump.None;

	private Jump JumpState
	{
		get => _playerJumpState;
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
			_playerJumpState = value;
		}
	}

	private float JumpStateTimer = -1;
	public CountUpTimer QueueJumpTimer;
	public CountUpTimer QueueDodgeTimer;

	public void JumpMovement()
	{
		bool jumpKey = Input.GetKey(Settings.CurrentSettings.Jump);

		switch (JumpState)
		{
			case Jump.None:
				if (CanJump && !QueueJumpTimer.Check())// If press jump key and can jump
				{
					JumpState = Jump.PreJump; // Start jump animation
											  //FloatTimeLeft = FloatTimerMax;
					QueueJumpTimer.Force();
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
				//Velocity.Gravity = 0.1f * 9.81f;
				RigidBody.gravityScale = 0.1f;
				if (JumpStateTimer <= 0 || !jumpKey || OnGround)
				{
					JumpState = Jump.JumpDelay;
					//Velocity.Gravity = GravityScale * 9.81f;
					RigidBody.gravityScale = GravityScale;
				}
				break;

			/*			case Jump.Floating:
							if (!jumpKey || OnGround)
								JumpState = Jump.None;
							break;*/

			case Jump.JumpDelay:
				if (JumpStateTimer < 0)
					JumpState = Jump.None;
				break;
		}
	}

	public void DodgeMovement()
	{
		if (!InDodgeRails)
		{
			if (OnGround)
				DodgesLeft = maxDodges;

			if (DodgesLeft > 0 && !QueueDodgeTimer.Check())
			{
				DodgesLeft--;
				DodgeStateTimer = DodgeTime;
				InDodgeRails = true;
			}
		}

		if (InDodgeRails)
		{
			Velocity.SetVelocity(DodgeSpeedOverride * -FaceDirection * Vector2.right);

			if (DodgeStateTimer <= 0)
			{
				InDodgeRails = false;
			}
		}

		if (healthData != null)
			healthData.OtherImmune = IsDodgeImmune;
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
		=> (Input.GetKey(Settings.CurrentSettings.Left) || Input.GetKey(Settings.CurrentSettings.Right)) // If pressing movement key
			? (((Velocity.x < 0 && Input.GetKey(Settings.CurrentSettings.Right)) || (Velocity.x > 0 && Input.GetKey(Settings.CurrentSettings.Left))) // and if pressing the opposite direction to movement
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
			animationHelper.CheckedSwapToFrame(Dodge);
		}
		else if (JumpState == Jump.PreJump)
		{
			animationHelper.CheckedSwapToFrame(JumpFrames[2].Frame);
		}
		else if (RiseSpeedMet)
		{
			animationHelper.CheckedSwapToFrame(JumpFrames[0].Frame);
		}
		else if (!OnGround)
		{
			animationHelper.CheckedSwapToFrame(JumpFrames[1].Frame);
		}
		else if (OnGround)
		{
			if (RunningSpeedMet && JumpState == Jump.None)
				animationHelper.CheckedSwapToSequence(WalkFrames);
			else
				animationHelper.CheckedSwapToSequence(IdleFrames);
		}

		animationHelper.Update();

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

	private bool Dead;
	private float DeathTime = 5f;

	public override void OnDeath()
	{
		animationHelper.CheckedSwapToSequence(DeathFrames);
		animationHelper.LoopAnimation = false;
		animationHelper.FreezeChanges = true;
		if (!Dead)
			sword.DeathClatter();
		RigidBody.gravityScale = GravityScale;
		Dead = true;
	}

	#endregion Death

	#region Editor

	private void OnDrawGizmosSelected()
	{
		if (SpriteRenderer == null)
			SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		Extensions.GizmosDrawRect(BoundsCheckingRect);
	}

	#endregion Editor

	public Rect BoundsCheckingRect => SpriteRenderer.bounds.ZFlattened();

	#region Persistance

	public PlayerData ReadData()
		=> new()
		{
			PlayerHealth = healthData.health,
			Items = Items
		};

	public void WriteData(PlayerData playerData)
	{
		if (playerData != null)
		{
			healthData.health = playerData.PlayerHealth;
			healthData.SupressReset();
			Items = playerData.Items;
		}
	}

	#endregion Persistance
}