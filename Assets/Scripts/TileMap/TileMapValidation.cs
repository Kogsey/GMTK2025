using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public static class TileMapValidation
{
	/// <summary> </summary>
	/// <param name="left"> Not null </param>
	/// <param name="right"> Allows null </param>
	public static void ValidateRooms(Room left, Room right)
	{
		if (right != null)
		{
			FixImpossibleHallHeight(left, right);
			FixImpossibleHallHeight(left, right);

			FixRoomShorterThanHall(right, left);
		}

		FixRoomShorterThanHall(left, left);

		LastCheckPrintMistakes(left, left.ConnectionBounds);

		if (right != null)
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

	public static void ValidateEntities(Tilemap tileMap, Room[] rooms)
	{
		foreach (Room room in rooms)
		{
			foreach (GameObject obj in room.EnumerateRoomObjects)
			{
				if (obj.TryGetComponent(out Light2D _) && obj.TryGetComponent(out SpriteRenderer spriteRenderer))
				{
					if (!IsValidLightPos(spriteRenderer, tileMap, rooms))
						Debug.LogWarning("Light Outside Bounds At " + spriteRenderer.bounds.ZFlattened().ToString());
				}
			}
		}
	}

	private static bool IsValidLightPos(SpriteRenderer light, Tilemap tileMap, Room[] rooms)
	{
		bool inAny = rooms.Any(room => IsContainedByRoomOrHall(light, tileMap, room));

		return inAny;
	}

	private static bool IsContainedByRoomOrHall(SpriteRenderer light, Tilemap tileMap, Room room)
	{
		Rect tileMapRect = tileMap.CellToWorld(room.RoomTilesBounds);
		Rect connectionRect = tileMap.CellToWorld(room.ConnectionBounds);
		Rect lightBounds = light.bounds.ZFlattened();
		return tileMapRect.Contains(lightBounds) || connectionRect.Contains(lightBounds);
	}
}