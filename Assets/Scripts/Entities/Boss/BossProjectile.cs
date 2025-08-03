using UnityEngine;

public class BossProjectile : MonoBehaviour
{
	public float Lifetime = 20f;
	private float LifeTimer = 0;

	public Sprite[] Sprites;
	public float TimePerFrame;
	public SpriteRenderer Renderer;
	public Collider2D Collider;
	public Rigidbody2D RigidBody;

	private AnimationHelper animationHelper;

	private void Awake()
	{
		if (Renderer == null)
			Renderer = GetComponent<SpriteRenderer>();
		if (Collider == null)
			Collider = GetComponent<Collider2D>();
		if (RigidBody == null)
			RigidBody = GetComponent<Rigidbody2D>();

		animationHelper = new(Renderer);
	}

	private void Start()
	{
		animationHelper.CheckedSwapToSequence(Sprites, TimePerFrame);
		animationHelper.LoopAnimation = true;
		animationHelper.LoopStartPoint = Sprites.Length - 2;
	}

	private Vector2 Velocity;

	public void SetShoot(Vector2 velocity)
		=> Velocity = velocity;

	private void Update()
	{
		if (Velocity.x > 0)
			transform.localScale = new Vector3(-1, 1, 1);

		animationHelper.Update();
		LifeTimer += Time.deltaTime;
		RigidBody.velocity = Velocity;

		if (LifeTimer > Lifetime)
			Destroy(gameObject);
	}
}