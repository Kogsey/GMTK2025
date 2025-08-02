using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileMapHelpers;
using Random = UnityEngine.Random;

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

	public LightHelper HangingLampPrefab;

	// Start is called before the first frame update
	private void Start()
		=> GenerateTileMap(1);

	private void OnDrawGizmosSelected()
	{
		//Draw the ground colliders on screen for debug purposes
		Gizmos.color = new Color(1f, 0.6470f, 0);
		Gizmos.DrawSphere(Foreground.CellToWorld(Vector3Int.zero), 0.1f);

		if (RoomsArray != null)
		{
			foreach (Room room in RoomsArray)
			{
				Extensions.GizmosDrawRect(Foreground.CellToWorld(room.RoomTilesBounds));
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

		for (int roomI = 0; roomI < RoomsArray.Length; roomI++)
		{
			Vector2Int size = TileMapRandom.GenRandBoxSize(level);
			RectInt rect = new(nextRoomGround, size);

			Room room = new()
			{
				Type = roomI == 0 ? Room.RoomType.Starting : Room.RoomType.Simple,
				RoomTilesBounds = rect,
				ConnectionGroundOffset = TileMapRandom.NextConnectionGroundOffset(),
				ConnectionHallHeight = TileMapRandom.NextConnectionHeight(),
				ConnectionLength = TileMapRandom.NextConnectionLength(),
			};

			RoomsArray[roomI] = room;

			if (roomI > 0)
				TileMapValidation.ValidateRooms(RoomsArray[roomI - 1], room);

			SetViewPaddingTiles(room);

			nextRoomGround.x = room.ConnectionBounds.xMax;
			nextRoomGround.y = room.ConnectionBounds.yMin - TileMapRandom.NextConnectionGroundOffset();
		}

		for (int roomI = 0; roomI < RoomsArray.Length; roomI++)
		{
			Room room = RoomsArray[roomI];
			SetRoomTiles(room);

			if (room.Type == Room.RoomType.Simple)
				GenRoomEnemies(level, roomI, room);

			GenRoomLamps(room);
		}

		// Go to until the last room since it has no connections
		for (int roomI = 0; roomI < RoomsArray.Length; roomI++)
		{
			// We do this at the end so we can overlap previous room walls
			Room room = RoomsArray[roomI];
			Room nextRoom = null;
			if (roomI + 1 < RoomsArray.Length)
				nextRoom = RoomsArray[roomI + 1];

			SetHallTiles(room, nextRoom);
			AddConnectionArrow(room);
		}

		Room room1 = RoomsArray[0];
		Player.transform.position = Foreground.CellToWorld(new Vector3Int((int)room1.RoomTilesBounds.center.x, (int)room1.RoomTilesBounds.center.y));

		TileMapValidation.ValidateEntities(Foreground, RoomsArray);
	}

	private void AddConnectionArrow(Room room)
	{
		SpriteRenderer newArrow = Instantiate(ArrowObject);
		newArrow.transform.position = Foreground.CellToWorld(room.ConnectionBounds).center;
		room.AddRoomObject(newArrow.gameObject);
	}

	/// <param name="prevRoom"> not null </param>
	/// <param name="nextRoom"> maybe null </param>
	private void SetHallTiles(Room prevRoom, Room nextRoom)
	{
		RectInt connection = prevRoom.ConnectionBounds;
		SetXEdges(Foreground, connection, TileSet.EdgeTiles);
		SetYEdges(Foreground, connection, TileSet.AirEdges);

		CornerTiles cornerTiles = TileSet.OuterCorners;

		if (prevRoom.RoomTilesBounds.yMin == connection.yMin)
			cornerTiles.BottomLeft = TileSet.EdgeTiles.Bottom;
		if (nextRoom == null || nextRoom.RoomTilesBounds.yMin == connection.yMin)
			cornerTiles.BottomRight = TileSet.EdgeTiles.Bottom;

		if (prevRoom.RoomTilesBounds.yMax == connection.yMax)
			cornerTiles.TopLeft = TileSet.EdgeTiles.Top;
		if (nextRoom == null || nextRoom.RoomTilesBounds.yMax == connection.yMax)
			cornerTiles.TopRight = TileSet.EdgeTiles.Top;

		SetCorners(Foreground, connection, cornerTiles);
		ClearFill(Foreground, connection.Inflate(0, -1));
	}

	private void SetRoomTiles(Room room)
	{
		RectInt roomBounds = room.RoomTilesBounds;

		SetCorners(Foreground, roomBounds, TileSet.InnerCorners);
		SetXEdges(Foreground, roomBounds, TileSet.EdgeTiles);
		SetYEdges(Foreground, roomBounds, TileSet.EdgeTiles);
		ClearFill(Foreground, roomBounds.Inflate(-1));
	}

	private const int ViewPadding = 16;

	private void SetViewPaddingTiles(Room room)
		=> BackdropFill(Foreground, room.RoomTilesBounds.Inflate(ViewPadding), TileSet);

	private void GenRoomEnemies(int level, int _, Room room)
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
				enemyBehave.RoomArea = Foreground.CellToWorld(room.RoomTilesBounds);
				enemyBehave.transform.position = Extensions.RandomIn(enemyBehave.RoomArea);
			}
			else
			{
				throw new NotImplementedException();
			}

			room.AddRoomObject(enemyBehave.gameObject);
		}
	}

	[Header("Lighting")]
	public float MinLampsPerTile;

	public float MaxLampsPerTile;

	public float MinLampsPerTileBoss;
	public float MaxLampsPerTileBoss;

	private void GenRoomLamps(Room room)
	{
		float minPT = MinLampsPerTile;
		float maxPT = MaxLampsPerTile;

		if (room.Type == Room.RoomType.Boss)
		{
			minPT = MinLampsPerTileBoss;
			maxPT = MaxLampsPerTileBoss;
		}

		GenAreaLamps(room, room.RoomTilesBounds.Inflate(-1), minPT, maxPT);
		GenAreaLamps(room, room.ConnectionBounds.Inflate(-1), minPT, maxPT);
	}

	private void GenAreaLamps(Room room, RectInt tilesBounds, float minPT, float maxPT)
	{
		Rect bounds = Foreground.CellToWorld(tilesBounds);

		float minLamps = tilesBounds.width * minPT;
		float maxLamps = tilesBounds.width * maxPT;

		float lamps = Random.Range(minLamps, maxLamps);
		int rounded = Mathf.CeilToInt(lamps);
		for (int i = 0; i < rounded; i++)
		{
			LightHelper light = Instantiate(HangingLampPrefab);
			Vector2 lightSize = light.SpriteRenderer.bounds.size;
			float lightPPU = light.SpriteRenderer.sprite.pixelsPerUnit;
			//Vector2 lightPivot = light.SpriteRenderer.sprite.pivot * lightPPU;
			//float lightTopOffset = lightSize.y - lightPivot.y;

			light.transform.position = GetLightPosition(bounds, lightSize, lightPPU);
			float chainLength = bounds.yMax - light.SpriteRenderer.bounds.max.y;
			light.SetChainHeight(chainLength);

			room.AddRoomObject(light.gameObject);
		}
	}

	private static Vector2 GetLightPosition(Rect roomBounds, Vector2 lightSize, float PPU)
	{
		float pixelPadding = 3f / PPU;

		float xPadding = (lightSize.x / 2) + pixelPadding;
		float yPadding = (lightSize.y / 2) + pixelPadding;
		float x = Random.Range(roomBounds.xMin + xPadding, roomBounds.xMax - xPadding);
		float y = Extensions.RandomGaussianMinMax(roomBounds.yMin + yPadding, roomBounds.yMax - yPadding);

		return Extensions.PixelPerfectRound(new Vector2(x, y), PPU);
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