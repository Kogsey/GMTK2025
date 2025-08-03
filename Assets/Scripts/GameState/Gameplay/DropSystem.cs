using UnityEngine;

public class DropSystem : BetterSingleton<DropSystem>
{
	public float DropChance = 0.1f;
	public ItemDropCategory[] ItemDrops;
	public FloorItem DropPrefab;

	private ItemDrop GenNewDrop(int level, EnemyBehave enemyBehave)
	{
		ItemDropCategory chosenCategory = WeightedRandom.Pick(ItemDrops, drop => drop.Weight);
		return chosenCategory.RollNew(level);
	}

	public void DropNewFrom(int level, EnemyBehave enemyBehave)
	{
		FloorItem newItem = Instantiate(DropPrefab, enemyBehave.BoundsCheckingRect.center, Quaternion.identity);
		ItemDrop drop = GenNewDrop(level, enemyBehave);
		newItem.SetItem(drop);
	}

	public void DropCheck(EnemyBehave enemyBehave)
	{
		int level = BetterSingleton<GameplayLoop>.Instance.Level;

		if (Random.value <= DropChance)
			DropNewFrom(level, enemyBehave);
	}
}