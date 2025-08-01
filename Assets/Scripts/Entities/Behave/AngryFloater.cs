// Ignore Spelling: Collider

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AngryFloater : EnemyBehave
{
	public float MoveSpeed = 1f;
	private const float DeadGravity = 1f;

	protected override void InternalUpdate()
	{
		base.InternalUpdate();

		if (Dead)
		{
			RigidBody.drag = 1f;
		}
		else
		{
			if (TargetVector.magnitude < MoveSpeed / 10)
			{
				RigidBody.velocity = Vector3.zero;
				RigidBody.transform.position = TargetPos;
			}
			else
			{
				RigidBody.velocity = TargetVector.normalized * MoveSpeed;
			}
		}
	}

	private readonly float SpinCount = 10f;
	private readonly float SpinSpeedRadians = Mathf.PI * 5;
	private float Spin;

	public override void OnHit(EntityBehave hurtingMe)
		=> StartCoroutine(SpinOutAnimation());

	private IEnumerator SpinOutAnimation()
	{
		RigidBody.constraints &= RigidbodyConstraints2D.FreezeRotation;

		while (Spin < SpinCount * 2 * Mathf.PI)
		{
			Spin += SpinSpeedRadians * Time.deltaTime;
			RigidBody.rotation = Spin * Mathf.Rad2Deg;
			yield return null;
		}

		RigidBody.rotation = 0;
		Spin = 0;
		RigidBody.constraints |= RigidbodyConstraints2D.FreezeRotation;
	}

	public override void OnDeath()
	{
		base.OnDeath();
		RigidBody.gravityScale = DeadGravity;
		if (TryGetComponent(out Collider2D collision))
			collision.isTrigger = false;
	}
}