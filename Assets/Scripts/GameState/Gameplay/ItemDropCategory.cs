using System;
using UnityEngine;

public enum ItemDropType
{
	IncreasedHealth,
	FasterAttack,
	IncreasedDamage,
	HealthRegen,
	LongerDodge,
}

[Serializable]
public class ItemDropCategory
{
	public ItemDropType Type;
	public Sprite[] Sprites;

	public float Mean;

	/// <summary> Clamps to between -3 * Sigma and +3 * Sigma around mean </summary>
	public float Sigma;

	public float MeanPerLevel;
	public float Weight;

	public string Description;
	public string LongName;

	public ItemDrop RollNew(int level)
	{
		float currentMean = Mean + (level * MeanPerLevel);
		float roll = Extensions.RandomGaussianStdDev(currentMean, Sigma);
		return new()
		{
			Category = Type,
			Value = roll,
		};
	}
}

public class ItemDrop
{
	public ItemDropType Category;
	public float Value;
}