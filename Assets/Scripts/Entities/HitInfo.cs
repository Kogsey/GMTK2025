using System;

[Serializable]
public struct HitInfo
{
	public int RawDamage;
	public int Pierce;

	private HitInfo(int rawDamage, int pierce)
	{
		RawDamage = rawDamage;
		Pierce = pierce;
	}

	public static HitInfo GenericDamage(int rawDamage)
		=> new(rawDamage, 0);
}

[Serializable]
public readonly struct DamageInfo
{
	public readonly bool Immune;
	public readonly bool Miss;
	public readonly int RawDamage;
	public readonly int TrueDamage;

	public DamageInfo(bool immunity, bool hit, int rawDamage, int trueDamage)
	{
		Immune = immunity;
		Miss = hit;
		RawDamage = rawDamage;
		TrueDamage = trueDamage;
	}

	public static DamageInfo GetMiss()
		=> new(false, true, 0, 0);

	public static DamageInfo GetImmune()
		=> new(true, false, 0, 0);

	public static DamageInfo GetHit(int rawDamage, int trueDamage)
		=> new(false, false, rawDamage, trueDamage);

	public readonly int Blocked => RawDamage - TrueDamage;
}