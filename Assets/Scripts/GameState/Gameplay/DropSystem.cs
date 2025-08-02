using UnityEngine;

public class DropSystem : MonoBehaviour
{
	public ItemDropCategory[] ItemDrops;
	public FloorItem DropPrefab;

	private ItemDrop GenNewDrop(int level, EnemyBehave enemyBehave)
	{
		ItemDropCategory chosenCategory = WeightedRandom.Pick(ItemDrops, drop => drop.Weight);
		return chosenCategory.RollNew(level);
	}

	public void DropNewFrom(int level, EnemyBehave enemyBehave)
	{
	}
}