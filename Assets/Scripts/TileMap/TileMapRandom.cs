using UnityEngine;
using static Extensions;
using Random = UnityEngine.Random;

public static class TileMapRandom
{
	/// <summary> Maximum amount of tiles the player can clear when jumping </summary>
	public const int MaxTilesJump = 5;

	/// <summary> Minimum tiles the player can fit through </summary>
	public const int MinPlayerGap = 4;

	public static readonly Vector2Int HomeBoxTopLeft = new(0, 0);

	private static RectInt LevelToRoomRange(int level)
	{
		Vector2Int standardMin = new(16, 10);
		Vector2Int standardRange = new(16, 4);

		standardRange += new Vector2Int(level * 4, level * 2);
		return new RectInt(standardMin, standardRange);
	}

	public static Vector2Int GenRandBoxSize(int level)
	{
		RectInt roomRanges = LevelToRoomRange(level);
		int x = (int)RandomGaussianMinMax(roomRanges.xMin, roomRanges.xMax);
		int y = (int)RandomGaussianMinMax(roomRanges.yMin, roomRanges.yMax);
		return new Vector2Int(x, y);
	}

	public static int RandomRoomCount(int level)
		=> (int)RandomGaussianStdDev(5 + (level / 3), 0.5f);

	/// <inheritdoc cref="Room.ConnectionLength"/>
	public static int NextConnectionLength()
		=> Random.Range(3, 6);

	/// <inheritdoc cref="Room.ConnectionHallHeight"/>
	public static int NextConnectionHeight()
		=> Random.Range(MinPlayerGap + 1, (2 * MinPlayerGap) + 1);

	/// <inheritdoc cref="Room.ConnectionGroundOffset"/>
	public static int NextConnectionGroundOffset()
		=> Random.Range(0, MaxTilesJump);
}

/*	public static float RoomSizeHelper(float mean, float exp)
	{
		float randGen = Random.Range(0f, 1f);
		//int sign = Math.Sign(randGen);
		//if (sign == -1)
		//	randGen = -randGen;

		float pow = MathF.Pow(randGen, exp);
		float result = mean * MathF.Exp(-0.5f * pow);
		//result = result * sign;
		return result < 3 ? 3
			: result > 200 ? 200
			: !float.IsFinite(result) ? throw new Exception()
			: result;
	}*/