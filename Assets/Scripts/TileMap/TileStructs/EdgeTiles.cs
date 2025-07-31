using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public struct EdgeTiles
{
	[SerializeField] public TileBase Top;
	[SerializeField] public TileBase Bottom;
	[SerializeField] public TileBase Left;
	[SerializeField] public TileBase Right;

	/// <summary>
	/// Returns this edge set as a corner set where each corner has been 'flattened' in the x direction. <br/>
	/// TopLeft = Top <br/>
	/// BottomLeft = Bottom <br/>
	/// BottomRight = Bottom <br/>
	/// TopRight = Top
	/// </summary>
	public CornerTiles FlattenX => new()
	{
		TopLeft = Top,
		BottomLeft = Bottom,
		BottomRight = Bottom,
		TopRight = Top,
	};

	/// <summary>
	/// Returns this edge set as a corner set where each corner has been 'flattened' in the y direction. <br/>
	/// TopLeft = Left <br/>
	/// BottomLeft = Left <br/>
	/// BottomRight = Right <br/>
	/// TopRight = Right
	/// </summary>
	public CornerTiles FlattenY => new()
	{
		TopLeft = Left,
		BottomLeft = Left,
		BottomRight = Right,
		TopRight = Right,
	};
}