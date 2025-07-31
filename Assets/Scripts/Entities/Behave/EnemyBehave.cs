using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyBehave : EntityBehave
{
	public HitInfo HitInfo;
	public Vector2 TargetPos { get; set; }
	public Rect RoomArea;

	public bool PlayerInRoom()
		=> RoomArea.Intersects(Singleton<PlayerController>.instance.BoundsCheckingRect);

	protected override void InternalUpdate()
	{
		base.InternalUpdate();
		if (PlayerInRoom())
		{
			TargetPos = Singleton<PlayerController>.instance.transform.position;
		}
	}

	public Vector2 TargetVector => TargetPos - (Vector2)transform.position;
}