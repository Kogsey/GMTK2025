using UnityEngine;
using UnityEngine.Tilemaps;
using static TileMapHelpers;

public class TileMapGenerator : MonoBehaviour
{
	public SpriteRenderer ArrowObject;
	public PlayerController Player;

	public Tilemap Foreground;
	public Tilemap Background;
	public TileSet TileSet;
	private Room[] RoomsArray;

	public EnemyBehave SkullPrefab;

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
		Generated = true;

		int rooms = TileMapRandom.RandomRoomCount(level);
		RoomsArray = new Room[rooms];
		Vector2Int nextRoomGround = TileMapRandom.HomeBoxTopLeft;

		for (int roomI = 0; roomI < rooms; roomI++)
		{
			Vector2Int size = TileMapRandom.GenRandBoxSize(level);
			RectInt rect = new(nextRoomGround, size);

			Room room = new()
			{
				Type = roomI == 0 ? Room.RoomType.Starting : Room.RoomType.Simple,
				RoomBounds = rect,
				ConnectionGroundOffset = TileMapRandom.NextConnectionGroundOffset(),
				ConnectionHallHeight = TileMapRandom.NextConnectionHeight(),
				ConnectionLength = TileMapRandom.NextConnectionLength(),
			};

			RoomsArray[roomI] = room;
			SetRoomTiles(room);
			GenRoomEnemies(level, roomI, room);

			nextRoomGround.x = room.ConnectionBounds.xMax;
			nextRoomGround.y = room.ConnectionBounds.yMin - TileMapRandom.NextConnectionGroundOffset();
		}

		// Go to until the last room since it has no connections
		for (int roomI = 0; roomI < rooms - 1; roomI++)
		{
			// We do this at the end so we can overlap previous room walls
			SetHallTiles(RoomsArray[roomI], RoomsArray[roomI + 1]);
			AddConnectionArrow(RoomsArray[roomI]);
		}

		Room room1 = RoomsArray[0];
		Player.transform.position = Foreground.CellToWorld(new Vector3Int((int)room1.RoomBounds.center.x, room1.RoomBounds.yMin + 1));
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
		BackdropFill(Background, connection.Inflate(0, -1), TileSet);
	}

	private void SetRoomTiles(Room room)
	{
		RectInt roomBounds = room.RoomBounds;

		SetCorners(Foreground, roomBounds, TileSet.InnerCorners);
		SetXEdges(Foreground, roomBounds, TileSet.EdgeTiles);
		SetYEdges(Foreground, roomBounds, TileSet.EdgeTiles);
		BackdropFill(Background, roomBounds.Inflate(-1), TileSet);
	}

	private void GenRoomEnemies(int level, int roomNumber, Room room)
	{
		int enemyCount = Random.Range(0, level + 2);
		for (int i = 0; i < enemyCount; i++)
		{
			EnemyBehave enemyBehave = Instantiate(SkullPrefab);
			enemyBehave.RoomArea = Foreground.CellToWorld(room.RoomBounds);
			enemyBehave.transform.position = Extensions.RandomIn(enemyBehave.RoomArea);
			room.AddRoomObject(enemyBehave.gameObject);
		}
	}
}