using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public abstract class EntityBehave : MonoBehaviour
{
	public HitType EntityType;
	protected Rigidbody2D RigidBody;
	protected SpriteRenderer SpriteRenderer;
	private Color _colour = Color.white;

	public Color BaseColour
	{
		get => _colour;
		set => SpriteRenderer.color = _colour = value;
	}

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