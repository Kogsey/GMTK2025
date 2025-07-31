using System;
using UnityEngine;
using Utilities.Timing;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EntityBehave), typeof(SpriteRenderer))]
public sealed class EntityHealth : MonoBehaviour
{
	public EntityBehave entity;
	public SpriteRenderer SpriteRenderer;

	public int health;
	public int MaxHealth;

	public CountDownTimer ImmunityTimer = new() { MaxTime = 0.4f };
	public bool Immune => !ImmunityTimer.Check();

	public int FlatDefence;
	public float DamageReduction;

	[Range(0, 100)]
	public float dodgeChange;

	private void Start()
	{
		if (entity == null)
			entity = GetComponent<EntityBehave>();
		if (SpriteRenderer == null)
			SpriteRenderer = GetComponent<SpriteRenderer>();
		health = MaxHealth;
	}

	private void Update() => UpdateImmunity();

	private int ImmunityFlasher;
	private void UpdateImmunity()
	{
		ImmunityTimer.Tick();
		ImmunityFlasher++;

		SpriteRenderer.color = Immune ? new Color(1, 0, 0, ImmunityFlasher % 2 == 0 ? 0.5f : 1f) : Color.white;
	}

	/// <summary> Returns true on successful hit </summary>
	public DamageInfo TryDealDamage(HitInfo hitInfo)
		=> Immune ? DamageInfo.GetImmune()
		: Random.Range(0f, 100f) > dodgeChange ? DealDamage(hitInfo)
		: DamageInfo.GetMiss();

	public DamageInfo DealDamage(HitInfo hitInfo)
	{
		int finalDefence = FlatDefence - hitInfo.Pierce;
		if (finalDefence < 0)
			finalDefence = 0;

		int finalDamage = hitInfo.RawDamage - Mathf.FloorToInt(hitInfo.RawDamage * DamageReduction) - finalDefence;
		health -= finalDamage;
		if (health <= 0)
			entity.OnDeath();

		ImmunityTimer.Reset();
		return DamageInfo.GetHit(finalDamage, finalDamage);
	}

	private void OnTriggerEnter2D(Collider2D collision)
		=> OnTrigger(collision);

	private void OnTriggerStay2D(Collider2D collision)
		=> OnTrigger(collision);

	private void OnTrigger(Collider2D collision)
	{
		if (collision != null)
		{
			if (collision.gameObject.TryGetComponent(out EnemyBehave other))
			{
				TryDealDamage(other.HitInfo);
			}
		}
	}
}