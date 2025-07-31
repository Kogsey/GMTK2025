using UnityEngine;

public class Room
{
	public enum RoomType
	{
		Starting,
		Simple,
		Boss,
	}

	public RoomType Type { get; set; }
	public RectInt RoomBounds { get; set; }

	/// <summary>
	/// Distance from the floor the connection is. Inclusive to rooms
	/// </summary>
	public int ConnectionGroundOffset { get; set; }
	/// <summary>
	/// Internal height of the hall. Inclusive to rooms
	/// </summary>
	public int ConnectionHallHeight { get; set; }
	/// <summary>
	/// Length of the hall. Inclusive to rooms
	/// </summary>
	public int ConnectionLength { get; set; }

	public RectInt ConnectionBounds
	{
		get
		{
			Vector2Int position = new (RoomBounds.xMax, RoomBounds.yMin + ConnectionGroundOffset);
			Vector2Int size = new(ConnectionLength, ConnectionHallHeight);
			return new(position, size);
		}
	}
}