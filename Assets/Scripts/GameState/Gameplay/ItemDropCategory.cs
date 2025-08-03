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
			Parent = this,
			Value = roll,
		};
	}
}

public interface IPlayerStatEffector
{
	/// <summary> Called every stat regen </summary>
	void ApplyConstantEffect(PlayerController playerController);

	/// <summary> Called Once when the item is picked up </summary>
	void ApplyInstantEffect(PlayerController playerController);
}

public class ItemDrop : IPlayerStatEffector
{
	public ItemDropCategory Parent;
	public float Value;

	public void ApplyInstantEffect(PlayerController playerController)
	{
		switch (Parent.Type)
		{
			case ItemDropType.HealthRegen:
				playerController.AddHealthRegen((int)Value);
				break;

			case ItemDropType.IncreasedHealth:
				playerController.AddHealthRegen((int)Value, true);
				break;

			default:
				break;
		}
	}

	public void ApplyConstantEffect(PlayerController playerController)
	{
		switch (Parent.Type)
		{
			case ItemDropType.IncreasedHealth:
				playerController.MaxHealth += (int)Value;
				break;

			case ItemDropType.FasterAttack:
				playerController.AttackSpeed *= Value;
				break;

			case ItemDropType.IncreasedDamage:
				playerController.DamageMultiplier += Value;
				break;

			case ItemDropType.HealthRegen:
				break;

			case ItemDropType.LongerDodge:
				playerController.DodgeSpeed *= Value;
				break;

			default:
				break;
		}
	}
}