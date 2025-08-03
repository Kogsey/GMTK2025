using UnityEngine;

public class Bat : EnemyBehave
{
	private Animator animator;
	private const float DeadGravity = 1f;

	public float TrackingImpulse = 3f;
	public float FlapImpulse = 5.19f;
	public float BelowPlayerExtraImpulse = 1f;
	public float AbovePlayerLostImpulse = 1f;
	private readonly float[] FlapTimes = { 0.3f, 0.8f };

	protected override void InternalStart()
	{
		base.InternalStart();
		animator = GetComponent<Animator>();
	}

	protected override void InternalUpdate()
	{
		base.InternalUpdate();

		if (!Dead)
		{
			if (FlapCheck())
			{
				RigidBody.velocity += FlapImpulse * Vector2.up;

				if (HasTarget)
				{
					if (transform.position.y < TargetPos.y)
						RigidBody.velocity += BelowPlayerExtraImpulse * Vector2.up;
					if (transform.position.y > TargetPos.y)
						RigidBody.velocity -= AbovePlayerLostImpulse * Vector2.up;

					RigidBody.velocity += TrackingImpulse * TargetVector.normalized;
				}
			}
		}
	}

	int lookingForFlap = 0;

	private bool FlapCheck()
	{
		bool shouldFlap = false;
		AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);
		float currentTime = animatorState.normalizedTime % 1;

		if (lookingForFlap == 1)
		{
			shouldFlap = currentTime > FlapTimes[1] || currentTime < FlapTimes[0];
		}

		if (lookingForFlap == 0)
		{
			shouldFlap = currentTime < FlapTimes[1] && currentTime > FlapTimes[0];
		}

		if (shouldFlap)
			lookingForFlap = (lookingForFlap + 1) % FlapTimes.Length;

		return shouldFlap;
	}

	public override void OnDeath()
	{
		base.OnDeath();
		RigidBody.gravityScale = DeadGravity;
		gameObject.layer = 3;
		RigidBody.drag = 1f;
		if (TryGetComponent(out Collider2D collision))
			collision.isTrigger = false;
	}

	public override void SetDifficultyChanges(int level, PlayerController playerController)
	{
		base.SetDifficultyChanges(level, playerController);
		TrackingImpulse *= Mathf.Pow(1.1f, level);

		float damageMultiplier = Mathf.Max(1, playerController.DamageMultiplier);

		float healthMultiplier = damageMultiplier * 0.9f;

		if (TryGetComponent(out EntityHealth entityHealth))
		{
			int newHealth = Mathf.CeilToInt(entityHealth.MaxHealth * healthMultiplier);
			entityHealth.SetMaxHealth(newHealth);
			entityHealth.Health = newHealth;
		}
	}
}