using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyBehave : EntityBehave
{
	public bool Dead;
	private float DeadTimer;
	private PlayerController Player => Singleton<PlayerController>.instance;

	public int SightRange = 20;

	public bool HasTarget { get; set; }
	public Vector2 TargetPos { get; set; }
	public Rect RoomArea;

	public bool PlayerInRoom()
		=> RoomArea.Overlaps(Player.BoundsCheckingRect);

	public bool CanSee(Vector2 pos)
		=> Vector2.Distance(transform.position, pos) <= SightRange;

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

		HasTarget = false;
		if (PlayerInRoom() || CanSee(Player.transform.position))
		{
			HasTarget = true;
			TargetPos = Player.transform.position;
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