using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public abstract class EntityBehave : MonoBehaviour
{
	public HitType EntityType;
	protected Rigidbody2D RigidBody;
	protected SpriteRenderer SpriteRenderer;
	protected Light2D BackLight;
	private Color _colour = Color.white;

	protected virtual int FlipMod => 1;

	/// <summary> -1 for left, 1 for right </summary>
	public int FaceDirection { get => (int)transform.localScale.x * FlipMod; set => transform.localScale = new Vector3(value * FlipMod, 1, 1); }

	public Rect BoundsCheckingRect => SpriteRenderer.bounds.ZFlattened();

	public Color BaseColour
	{
		get => _colour;
		set
		{
			SpriteRenderer.color = _colour = value;
			if (BackLight != null)
				BackLight.color = value;
		}
	}

	private void Start()
		=> InternalStart();

	protected virtual void InternalStart()
	{ }

	private void Awake()
		=> InternalAwake();

	protected virtual void InternalAwake()
	{
		BackLight = GetComponentInChildren<Light2D>();
		RigidBody = GetComponent<Rigidbody2D>();
		SpriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
		=> InternalUpdate();

	protected virtual void InternalUpdate()
	{ }

	public virtual void OnHit(HitInfo hit)
	{
	}

	public virtual void OnHitOther(EntityBehave hurt)
	{
	}

	public abstract void OnDeath();
}