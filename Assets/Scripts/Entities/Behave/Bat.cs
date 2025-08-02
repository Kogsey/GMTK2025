// Ignore Spelling: Collider

using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Bat : EnemyBehave
{
	private Animator animator;
	private const float DeadGravity = 1f;

	public float TrackingImpulse = 5f;
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
}