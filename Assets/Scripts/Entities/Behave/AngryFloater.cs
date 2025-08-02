// Ignore Spelling: Collider

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AngryFloater : EnemyBehave
{
	public float MoveSpeed = 1f;
	private const float DeadGravity = 1f;

	public float BobSpeed = 1f;
	public float BobSpeedVariance = 1f;
	private float trueBobSpeed;
	public float BobMagnitude = 0.5f;

	protected override void InternalStart()
	{
		base.InternalStart();

		trueBobSpeed = BobSpeed + Random.Range(-BobSpeedVariance, BobSpeedVariance);
	}

	protected override void InternalUpdate()
	{
		base.InternalUpdate();

		if (Dead)
		{
			RigidBody.drag = 1f;
		}
		else
		{
			RigidBody.velocity = HasTarget ? TargetVector.normalized * MoveSpeed : Vector2.zero;
			RigidBody.velocity += BobMagnitude * Mathf.Sin(Time.time * trueBobSpeed) * Vector2.up;
		}
	}

	private readonly float SpinCount = 2f;
	private readonly float SpinSpeedRadians = Mathf.PI * 5;
	private float Spin;

	public override void OnHit(HitInfo hit)
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
		gameObject.layer = 3;
		if (TryGetComponent(out Collider2D collision))
			collision.isTrigger = false;
	}
}