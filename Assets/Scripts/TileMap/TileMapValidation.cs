using System;
using UnityEngine;

public static class TileMapValidation
{
	public static void ValidateRooms(Room left, Room right)
	{
		FixImpossibleHallHeight(left, right);
		FixImpossibleHallHeight(left, right);

		FixRoomShorterThanHall(left, left);
		FixRoomShorterThanHall(right, left);

		LastCheckPrintMistakes(left, left.ConnectionBounds);
		LastCheckPrintMistakes(right, left.ConnectionBounds);
	}

	private static void FixImpossibleHallHeight(Room left, Room right)
	{
		int maxHallHeight = GetMaxHalLHeight(left, right);
		if (maxHallHeight < TileMapRandom.MinPlayerGap)
		{
			int errorAmount = maxHallHeight - TileMapRandom.MinPlayerGap;

			Room badRoom = left.RoomTilesBounds.yMax < right.RoomTilesBounds.yMax ? left : right;
			RectInt roomBounds = badRoom.RoomTilesBounds;
			roomBounds.yMax -= errorAmount;
			badRoom.RoomTilesBounds = roomBounds;
		}
	}

	private static int GetMaxHalLHeight(Room left, Room right)
	{
		// Lowest valid hall pos
		int trueMinY = Math.Max(left.RoomTilesBounds.yMin, right.RoomTilesBounds.yMin);
		// Highest valid hall pos
		int trueMaxY = Math.Min(left.RoomTilesBounds.yMax, right.RoomTilesBounds.yMax);

		return trueMaxY - trueMinY;
	}

	private static void FixRoomShorterThanHall(Room room, Room roomWithConnection)
	{
		if (room.RoomTilesBounds.yMax < roomWithConnection.ConnectionBounds.yMax)
		{
			roomWithConnection.ConnectionHallHeight -= roomWithConnection.ConnectionBounds.yMax - room.RoomTilesBounds.yMax;
		}
	}

	private static void LastCheckPrintMistakes(Room left, RectInt connection)
	{
		if (left.RoomTilesBounds.yMax < connection.yMax)
			Debug.LogWarning("Hall Spawned Too Tall.");
		if (left.RoomTilesBounds.yMin > connection.yMin)
			Debug.LogWarning("Room Spawned Too High.");
	}
}