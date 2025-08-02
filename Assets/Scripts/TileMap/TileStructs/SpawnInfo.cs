using System;
using UnityEngine;

[Flags]
public enum SpawnFlags
{
	None = 0,
	Floater = 1,

	Boss = 4,
}

[Serializable]
public struct MinMax
{
	public int Min;
	public int Max;
}

[Serializable]
public class SpawnInfo
{
	public MinMax PrimarySpawnCount;
	public MinMax AsSupportCount;
	public float PrimarySpawnWeight;
	public float SupportSpawnWeight;

	public float ExtraWeightPerLevelPrimary;
	public float ExtraWeightPerLevelSupport;

	public float ExtraSpawnsPerLevelPrimary;
	public float ExtraSpawnsPerLevelSupport;

	public SpawnFlags Flags;
	public bool CanBeSupport => AsSupportCount.Max > 0 && SupportSpawnWeight > 0;
	public bool CanBePrimary => PrimarySpawnCount.Max > 0 && PrimarySpawnWeight > 0;

	public EnemyBehave Prefab;
}