using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

public class Room
{
	public enum RoomType
	{
		Starting,
		Simple,
		Boss,
	}

	public RoomType Type { get; set; }

	/// <summary> Between 0 and 1 </summary>
	public float LightIntensity { get; set; }

	public int RoomTilesWidth { get; set; }
	public int RoomTilesHeight { get; set; }
	public int RoomTilesX { get; set; }
	public int RoomTilesY { get; set; }

	public RectInt RoomTilesBounds
	{
		get => new(RoomTilesX, RoomTilesY, RoomTilesWidth, RoomTilesHeight);
		set
		{
			RoomTilesX = value.x;
			RoomTilesY = value.y;
			RoomTilesWidth = value.width;
			RoomTilesHeight = value.height;
		}
	}

	public Rect GetWorldBounds(Tilemap tileMap)
		=> tileMap.CellToWorld(RoomTilesBounds);

	/// <summary> Distance from the floor the connection is. Inclusive to rooms </summary>
	public int ConnectionGroundOffset { get; set; }

	/// <summary> Internal height of the hall. Inclusive to rooms </summary>
	public int ConnectionHallHeight { get; set; }

	/// <summary> Length of the hall. Inclusive to rooms </summary>
	public int ConnectionLength { get; set; }

	private List<GameObject> RoomObjects { get; set; } = new List<GameObject>();

	public IEnumerable<GameObject> EnumerateRoomObjects => RoomObjects;

	public void AddRoomObject(GameObject gameObject)
		=> RoomObjects.Add(gameObject);

	public void CleanRoom()
	{
		RoomObjects.ForEach(Object.Destroy);
		RoomObjects.Clear();
	}

	public RectInt ConnectionBounds
	{
		get
		{
			Vector2Int position = new(RoomTilesBounds.xMax, RoomTilesBounds.yMin + ConnectionGroundOffset);
			Vector2Int size = new(ConnectionLength, ConnectionHallHeight);
			return new(position, size);
		}
	}

	//public void SetHallYMaxByHeight(int yMax)
	//	=> ConnectionHallHeight = yMax - (RoomBounds.yMin + ConnectionGroundOffset);
}