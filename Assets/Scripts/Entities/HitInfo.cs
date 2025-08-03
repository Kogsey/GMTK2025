using System;
using UnityEngine;

/// <summary> What this hit can hit </summary>
[Flags]
public enum HitType
{
	Impotent = 0,
	Enemy = 1,
	Player = 2,
	Generic = Enemy | Player,
}

[Serializable]
public struct HitInfo
{
	public HitType HitType;
	public int RawDamage;
	public int Pierce;

	private HitInfo(HitType hitType, int rawDamage, int pierce)
	{
		HitType = hitType;
		RawDamage = rawDamage;
		Pierce = pierce;
	}

	public static HitInfo GenericDamage(int rawDamage)
		=> new(HitType.Generic, rawDamage, 0);

	public static HitInfo GetImpotent()
		=> new(HitType.Impotent, 0, 0);

	public readonly HitInfo WithMultiplier(float multiply)
		=> new(HitType, Mathf.CeilToInt(RawDamage * multiply), Pierce);
}

public enum DamageResult
{
	/// <summary> Hit entity does not take damage </summary>
	Immune,

	/// <summary> Hit was successful </summary>
	Hit,

	/// <summary> Resulting damage was 0 </summary>
	Defended,

	/// <summary> Damage was of incorrect <see cref="HitType"/> </summary>
	Ignored,

	/// <summary> Damage was dodged or missed </summary>
	Miss,
}

[Serializable]
public readonly struct DamageInfo
{
	public readonly DamageResult DamageResult;
	public readonly int RawDamage;
	public readonly int TrueDamage;

	private DamageInfo(DamageResult damageResult, int rawDamage, int trueDamage)
	{
		DamageResult = damageResult;
		RawDamage = rawDamage;
		TrueDamage = trueDamage;
	}

	public static DamageInfo GetSimpleResult(DamageResult damageResult)
		=> new(damageResult, 0, 0);

	public static DamageInfo GetHit(int rawDamage, int trueDamage)
		=> new(DamageResult.Hit, rawDamage, trueDamage);

	public readonly int Blocked => RawDamage - TrueDamage;
}