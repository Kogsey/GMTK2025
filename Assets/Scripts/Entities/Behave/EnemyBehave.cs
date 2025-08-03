using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyBehave : EntityBehave
{
	public bool Dead;
	private float DeadTimer;
	protected PlayerController Player => FindAnyObjectByType<PlayerController>();

	public int SightRange = 20;
	public int AlreadySeenSightRange = 20;

	private bool _hasTarget;
	public virtual bool HasTarget => _hasTarget;
	private Vector2 _targetPos;
	public virtual Vector2 TargetPos => _targetPos;

	public virtual bool AutoFlip => true;
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
			BaseColour = Color.Lerp(Color.white, new Color(0, 0, 0, 0), DeadTimer / DeathTimerMax);
			if (DeadTimer >= DeathTimerMax)
				OnDeathTimerEnd();
		}

		if (PlayerInRoom() || CanSee(Player.transform.position))
		{
			_hasTarget = true;
			_targetPos = Player.transform.position;
		}
		else
		{
			_hasTarget = false;
		}

		if (AutoFlip)
		{
			flipTimer -= Time.deltaTime;
			int preferredFaceDirection = -(int)Mathf.Sign(RigidBody.velocityX);
			if (preferredFaceDirection != FaceDirection && (preferredFaceDirection == -1 || preferredFaceDirection == 1) && flipTimer <= 0)
			{
				flipTimer = maxFlipTimer;
				FaceDirection = preferredFaceDirection;
			}
		}
	}

	public override void OnDeath()
	{
		if (TryGetComponent(out Aggressor aggressor))
			aggressor.HitInfo = HitInfo.GetImpotent();

		BetterSingleton<DropSystem>.Instance.DropCheck(this);
		Dead = true;
	}

	protected virtual float DeathTimerMax => 1f;
	protected virtual void OnDeathTimerEnd() 
		=> Destroy(gameObject);

	public Vector2 TargetVector => TargetPos - (Vector2)transform.position;

	private void OnDrawGizmosSelected()
	{
		if (HasTarget)
			Gizmos.DrawSphere(Player.transform.position, 1);

		Gizmos.DrawSphere(transform.position, SightRange);
		Extensions.GizmosDrawRect(RoomArea);
	}
}