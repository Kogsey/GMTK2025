using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Utilities.Timing;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EntityBehave), typeof(SpriteRenderer))]
public sealed class EntityHealth : MonoBehaviour
{
	public HealthBar HealthBar;
	public EntityBehave entity;
	public SpriteRenderer SpriteRenderer;
	public Collider2D[] DamageMeColliders;

	public bool CheatHealth;

	public int _health;
	public int Health
	{
		get => _health;
		set
		{
			_health = value;
			HealthBar.UpdateHealth(_health);
		}
	}

	public int MaxHealth;

	public CountDownTimer ImmunityTimer = new() { MaxTime = 0.4f };
	public bool OtherImmune { get; set; }
	public bool InImmunityFrames => !ImmunityTimer.Check();

	public bool AnyImmune => OtherImmune || InImmunityFrames;

	public int FlatDefence;
	public float DamageReduction;

	[Range(0, 100)]
	public float dodgeChange;

	private void Awake()
	{
		if (entity == null)
			entity = GetComponent<EntityBehave>();
		if (SpriteRenderer == null)
			SpriteRenderer = GetComponent<SpriteRenderer>();
		if (DamageMeColliders == null || DamageMeColliders.Length == 0)
			DamageMeColliders = GetComponents<Collider2D>();
		if (HealthBar == null)
			HealthBar = GetComponentInChildren<HealthBar>();
	}

	private void Start()
	{
		if (!resetSupressed)
			_health = MaxHealth;

		HealthBar.UpdateAll(MaxHealth, Health);
	}

	private void Update()
	{
		if (CheatHealth)
			_health = MaxHealth;
		UpdateImmunity();
	}

	private bool resetSupressed;

	/// <summary> Stops health being set to maxHealth on start </summary>
	public void SupressReset()
		=> resetSupressed = true;

	public void SetMaxHealth(int newMaxHealth)
	{
		MaxHealth = newMaxHealth;
		HealthBar.UpdateAll(MaxHealth, Health);
	}

	public void UpdateMaxHealthByPercent(int newMaxHealth)
	{
		if (newMaxHealth != MaxHealth)
		{
			float oldPercent = (float)MaxHealth / Health;
			MaxHealth = newMaxHealth;
			Health = Mathf.CeilToInt(oldPercent * MaxHealth);
			HealthBar.UpdateAll(MaxHealth, Health);
		}
	}

	public void ClampHealth()
		=> Health = math.clamp(Health, 0, MaxHealth);

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
		_health -= finalDamage;

		if (HealthBar != null)
			HealthBar.UpdateHealth(_health);

		if (_health <= 0)
			entity.OnDeath();

		entity.OnHit(hitInfo);
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
				_ = TryDealDamage(other.HitInfo);
			}
		}
	}
}