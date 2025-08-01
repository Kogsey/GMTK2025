using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyBehave : EntityBehave
{
	public bool Dead;
	private float DeadTimer;

	public Vector2 TargetPos { get; set; }
	public Rect RoomArea;

	public bool PlayerInRoom()
		=> RoomArea.Intersects(Singleton<PlayerController>.instance.BoundsCheckingRect);

	protected override void InternalUpdate()
	{
		base.InternalUpdate();
		if (Dead)
		{
			DeadTimer += Time.deltaTime;
			BaseColour = Color.Lerp(Color.white, new Color(0, 0, 0, 0), DeadTimer);
			if (DeadTimer >= 1)
				Destroy(gameObject);
		}

		if (PlayerInRoom())
		{
			TargetPos = Singleton<PlayerController>.instance.transform.position;
		}
	}

	public override void OnDeath()
		=> Dead = true;

	public Vector2 TargetVector => TargetPos - (Vector2)transform.position;
}