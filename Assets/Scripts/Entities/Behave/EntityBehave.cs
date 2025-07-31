using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public abstract class EntityBehave : MonoBehaviour
{
	protected Rigidbody2D RigidBody;
	protected SpriteRenderer SpriteRenderer;

	private void Start()
		=> InternalStart();

	protected virtual void InternalStart()
	{
		RigidBody = GetComponent<Rigidbody2D>();

		SpriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
		=> InternalUpdate();

	protected virtual void InternalUpdate()
	{ }

	public virtual void OnHit(EntityBehave hurtingMe)
	{
	}

	public virtual void OnHitOther(EntityBehave hurt)
	{
	}

	public abstract void OnDeath();
}