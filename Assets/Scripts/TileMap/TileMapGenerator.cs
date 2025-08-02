using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using static TileMapHelpers;

public class TileMapGenerator : MonoBehaviour
{
	public SpriteRenderer ArrowObject;
	public PlayerController Player;

	public Tilemap Foreground;
	public Tilemap Background;
	public TileSet TileSet;
	private Room[] RoomsArray;

	public SpawnInfo[] Enemies;
	public float SupportEnemiesChance = 0.25f;

	// Start is called before the first frame update
	private void Start()
		=> GenerateTileMap(1);

	private void OnDrawGizmos()
	{
		//Draw the ground colliders on screen for debug purposes
		Gizmos.color = new Color(1f, 0.6470f, 0);
		Gizmos.DrawSphere(Foreground.CellToWorld(Vector3Int.zero), 0.1f);

		if (RoomsArray != null)
		{
			foreach (Room room in RoomsArray)
			{
				Extensions.GizmosDrawRect(Foreground.CellToWorld(room.RoomBounds));
				Extensions.GizmosDrawRect(Foreground.CellToWorld(room.ConnectionBounds));
			}
		}
	}

	private void UnGenerate()
	{
		foreach (Room room in RoomsArray)
			room.CleanRoom();

		RoomsArray = null;
		Foreground.ClearAllTiles();
		Background.ClearAllTiles();
	}

	private bool Generated = false;

	public void GenerateTileMap(int level)
	{
		if (Generated)
			UnGenerate();

		RegenRandoms(level);
		Generated = true;

		int rooms = TileMapRandom.RandomRoomCount(level);
		RoomsArray = new Room[rooms];
		Vector2Int nextRoomGround = TileMapRandom.HomeBoxTopLeft;
		int nextRoomMinY = -1;

		for (int roomI = 0; roomI < rooms; roomI++)
		{
			Vector2Int size = TileMapRandom.GenRandBoxSize(level);
			RectInt rect = new(nextRoomGround, size);
			if (rect.yMax < nextRoomMinY)
				rect.yMax = nextRoomMinY;

			Room room = new()
			{
				Type = roomI == 0 ? Room.RoomType.Starting : Room.RoomType.Simple,
				RoomBounds = rect,
				ConnectionGroundOffset = TileMapRandom.NextConnectionGroundOffset(),
				ConnectionHallHeight = TileMapRandom.NextConnectionHeight(),
				ConnectionLength = TileMapRandom.NextConnectionLength(),
			};

			RoomsArray[roomI] = room;

			SetViewPaddingTiles(room);

			nextRoomGround.x = room.ConnectionBounds.xMax;
			nextRoomGround.y = room.ConnectionBounds.yMin - TileMapRandom.NextConnectionGroundOffset();
			nextRoomMinY = room.ConnectionBounds.yMax;
		}

		for (int roomI = 0; roomI < rooms; roomI++)
		{
			Room room = RoomsArray[roomI];
			SetRoomTiles(room);
			GenRoomEnemies(level, roomI, room);
		}

		// Go to until the last room since it has no connections
		for (int roomI = 0; roomI < rooms - 1; roomI++)
		{
			// We do this at the end so we can overlap previous room walls
			Room room = RoomsArray[roomI];
			SetHallTiles(room, RoomsArray[roomI + 1]);
			AddConnectionArrow(room);
		}

		Room room1 = RoomsArray[0];
		Player.transform.position = Foreground.CellToWorld(new Vector3Int((int)room1.RoomBounds.center.x, (int)room1.RoomBounds.center.y));
	}

	private void AddConnectionArrow(Room room)
	{
		SpriteRenderer newArrow = Instantiate(ArrowObject);
		newArrow.transform.position = Foreground.CellToWorld(room.ConnectionBounds).center;
		room.AddRoomObject(newArrow.gameObject);
	}

	private void SetHallTiles(Room prevRoom, Room nextRoom)
	{
		RectInt connection = prevRoom.ConnectionBounds;
		SetXEdges(Foreground, connection, TileSet.EdgeTiles);
		SetYEdges(Foreground, connection, TileSet.AirEdges);

		CornerTiles cornerTiles = TileSet.OuterCorners;

		if (prevRoom.RoomBounds.yMin == connection.yMin)
			cornerTiles.BottomLeft = TileSet.EdgeTiles.Bottom;
		if (nextRoom.RoomBounds.yMin == connection.yMin)
			cornerTiles.BottomRight = TileSet.EdgeTiles.Bottom;

		if (prevRoom.RoomBounds.yMax == connection.yMax)
			cornerTiles.TopLeft = TileSet.EdgeTiles.Top;
		if (nextRoom.RoomBounds.yMax == connection.yMax)
			cornerTiles.TopRight = TileSet.EdgeTiles.Top;

		SetCorners(Foreground, connection, cornerTiles);
		ClearFill(Foreground, connection.Inflate(0, -1));
	}

	private void SetRoomTiles(Room room)
	{
		RectInt roomBounds = room.RoomBounds;

		SetCorners(Foreground, roomBounds, TileSet.InnerCorners);
		SetXEdges(Foreground, roomBounds, TileSet.EdgeTiles);
		SetYEdges(Foreground, roomBounds, TileSet.EdgeTiles);
		ClearFill(Foreground, roomBounds.Inflate(-1));
	}

	private const int ViewPadding = 16;

	private void SetViewPaddingTiles(Room room)
	{
		BackdropFill(Foreground, room.RoomBounds.Inflate(ViewPadding), TileSet);
	}

	private void GenRoomEnemies(int level, int roomNumber, Room room)
	{
		(SpawnInfo primary, SpawnInfo support) = PickSpawnInfo();

		int min = primary.PrimarySpawnCount.Min;
		int max = primary.PrimarySpawnCount.Max;

		min += (int)(primary.ExtraSpawnsPerLevelPrimary * level);
		max += (int)(primary.ExtraSpawnsPerLevelPrimary * level);

		GenRoomEnemies_Inner(room, min, max, primary);

		if (support != null)
		{
			min = support.AsSupportCount.Min;
			max = support.AsSupportCount.Max;

			min += (int)(support.ExtraSpawnsPerLevelSupport * level);
			max += (int)(support.ExtraSpawnsPerLevelSupport * level);

			GenRoomEnemies_Inner(room, min, max, support);
		}
	}

	private void GenRoomEnemies_Inner(Room room, int minEnemies, int maxEnemies, SpawnInfo spawnInfo)
	{
		int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
		for (int i = 0; i < enemyCount; i++)
		{
			EnemyBehave enemyBehave = Instantiate(spawnInfo.Prefab);
			if (spawnInfo.Flags.HasFlag(SpawnFlags.Floater))
			{
				enemyBehave.RoomArea = Foreground.CellToWorld(room.RoomBounds);
				enemyBehave.transform.position = Extensions.RandomIn(enemyBehave.RoomArea);
			}
			else
				throw new NotImplementedException();

			room.AddRoomObject(enemyBehave.gameObject);
		}
	}

	private WeightedRandom<SpawnInfo> PrimaryWeightedRandom;
	private WeightedRandom<SpawnInfo> SupportWeightedRandom;

	private void RegenRandoms(int level)
	{
		PrimaryWeightedRandom = new();
		SupportWeightedRandom = new();

		foreach (SpawnInfo item in Enemies)
		{
			if (item.CanBePrimary)
				PrimaryWeightedRandom.AddElement(item.PrimarySpawnWeight + (item.ExtraWeightPerLevelPrimary * level), item);

			if (item.CanBeSupport)
				SupportWeightedRandom.AddElement(item.SupportSpawnWeight + (item.ExtraWeightPerLevelSupport * level), item);
		}
	}

	private (SpawnInfo Primary, SpawnInfo Support) PickSpawnInfo()
	{
		SpawnInfo primary = PrimaryWeightedRandom.Pick();
		SpawnInfo support = null;

		if (Random.value <= SupportEnemiesChance)
		{
			support = SupportWeightedRandom.Pick();
		}

		return (primary, support);
	}
}