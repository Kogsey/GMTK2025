using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyBehave : EntityBehave
{
	public bool Dead;
	private float DeadTimer;
	private PlayerController Player => Singleton<PlayerController>.instance;

	public int SightRange = 20;
	public int AlreadySeenSightRange = 20;

	public bool HasTarget { get; set; }
	public Vector2 TargetPos { get; set; }
	public Rect RoomArea;

	public float maxFlipTimer = 1f;
	public float flipTimer = 0;

	public bool PlayerInRoom()
		=> RoomArea.Overlaps(Player.BoundsCheckingRect);

	public bool CanSee(Vector2 pos)
	{
		float distance = Vector2.Distance(transform.position, pos);
		float activeSightRange = SightRange;
		if (HasTarget)
			activeSightRange = AlreadySeenSightRange;

		return distance <= activeSightRange;
	}

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

		if (PlayerInRoom() || CanSee(Player.transform.position))
		{
			HasTarget = true;
			TargetPos = Player.transform.position;
		}
		else
		{
			HasTarget = false;
		}

		flipTimer -= Time.deltaTime;
		int preferredFaceDirection = -(int)Mathf.Sign(RigidBody.velocityX);
		if (preferredFaceDirection != FaceDirection && (preferredFaceDirection == -1 || preferredFaceDirection == 1) && flipTimer <= 0)
		{
			flipTimer = maxFlipTimer;
			FaceDirection = preferredFaceDirection;
		}
	}

	public override void OnDeath()
	{
		if (TryGetComponent(out Aggressor aggressor))
			aggressor.HitInfo = HitInfo.GetImpotent();

		Dead = true;
	}

	public Vector2 TargetVector => TargetPos - (Vector2)transform.position;

	private void OnDrawGizmosSelected()
	{
		if (HasTarget)
			Gizmos.DrawSphere(Player.transform.position, 1);

		Gizmos.DrawSphere(transform.position, SightRange);
		Extensions.GizmosDrawRect(RoomArea);
	}
}