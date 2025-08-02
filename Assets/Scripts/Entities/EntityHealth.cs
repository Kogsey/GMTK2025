using System;
using System.Linq;
using UnityEngine;
using Utilities.Timing;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EntityBehave), typeof(SpriteRenderer))]
public sealed class EntityHealth : MonoBehaviour
{
	public EntityBehave entity;
	public SpriteRenderer SpriteRenderer;
	public Collider2D[] DamageMeColliders;

	public bool CheatHealth;
	private int health;
	public int MaxHealth;

	public CountDownTimer ImmunityTimer = new() { MaxTime = 0.4f };
	public bool OtherImmune { get; set; }
	public bool InImmunityFrames => !ImmunityTimer.Check();

	public bool AnyImmune => OtherImmune || InImmunityFrames;

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
		if (DamageMeColliders == null || DamageMeColliders.Length == 0)
			DamageMeColliders = GetComponents<Collider2D>();

		health = MaxHealth;
	}

	private void Update()
	{
		if (CheatHealth)
			health = MaxHealth;
		UpdateImmunity();
	}

	private int ImmunityFlasher;

	private Color FlashColour
		=> InImmunityFrames ? Color.red : Color.white;

	private void UpdateImmunity()
	{
		ImmunityTimer.Tick();
		ImmunityFlasher++;

		if (AnyImmune)
		{
			Color currentColour = FlashColour;
			currentColour.a = ImmunityFlasher % 2 == 0 ? 0.5f : 1f;
			SpriteRenderer.color = currentColour;
		}
		else
		{
			SpriteRenderer.color = entity.BaseColour;
		}
	}

	/// <summary> Returns true on successful hit </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Code Style")]
	public DamageInfo TryDealDamage(HitInfo hitInfo)
	{
		if ((hitInfo.HitType & entity.EntityType) == 0)
			return DamageInfo.GetSimpleResult(DamageResult.Ignored);

		if (AnyImmune) // If immune
			return DamageInfo.GetSimpleResult(DamageResult.Immune);
		if (Random.Range(0f, 100f) <= dodgeChange) // If dodged
			return DamageInfo.GetSimpleResult(DamageResult.Miss);
		return DealDamage(hitInfo);
	}

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
		if (collision != null && DamageMeColliders.Any(col => collision.IsTouching(col)))
		{
			if (collision.gameObject.TryGetComponent(out Aggressor other))
			{
				TryDealDamage(other.HitInfo);
			}
		}
	}
}