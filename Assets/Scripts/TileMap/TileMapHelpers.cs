using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileMapHelpers
{
	public static void SetCorners(Tilemap tileMap, RectInt bounds, CornerTiles cornerTiles)
	{
		SetTopLeftCorner(tileMap, bounds, cornerTiles);
		SetTopRightCorner(tileMap, bounds, cornerTiles);
		SetBottomLeftCorner(tileMap, bounds, cornerTiles);
		SetBottomRightCorner(tileMap, bounds, cornerTiles);
	}
	public static void SetBottomLeftCorner(Tilemap tileMap, RectInt bounds, CornerTiles cornerTiles)
		=> tileMap.SetTile(new Vector3Int(bounds.xMin, bounds.yMin), cornerTiles.BottomLeft);
	public static void SetBottomRightCorner(Tilemap tileMap, RectInt bounds, CornerTiles cornerTiles)
		=> tileMap.SetTile(new Vector3Int(bounds.xMax, bounds.yMin), cornerTiles.BottomRight);
	public static void SetTopLeftCorner(Tilemap tileMap, RectInt bounds, CornerTiles cornerTiles)
		=> tileMap.SetTile(new Vector3Int(bounds.xMin, bounds.yMax), cornerTiles.TopLeft);
	public static void SetTopRightCorner(Tilemap tileMap, RectInt bounds, CornerTiles cornerTiles)
		=> tileMap.SetTile(new Vector3Int(bounds.xMax, bounds.yMax), cornerTiles.TopRight);

	public static void SetXEdges(Tilemap tileMap, RectInt bounds, EdgeTiles edgeTiles)
	{
		for (int xPos = bounds.xMin + 1; xPos < bounds.xMax; xPos++)
		{
			tileMap.SetTile(new Vector3Int(xPos, bounds.yMin), edgeTiles.Bottom);
			tileMap.SetTile(new Vector3Int(xPos, bounds.yMax), edgeTiles.Top);
		}
	}

	public static void SetYEdges(Tilemap tileMap, RectInt bounds, EdgeTiles edgeTiles)
	{
		for (int yPos = bounds.yMin + 1; yPos < bounds.yMax; yPos++)
		{
			tileMap.SetTile(new Vector3Int(bounds.xMin, yPos), edgeTiles.Left);
			tileMap.SetTile(new Vector3Int(bounds.xMax, yPos), edgeTiles.Right);
		}
	}

	public static void BackdropFill(Tilemap tileMap, RectInt bounds, TileSet tileSet)
	{
		for (int xPos = bounds.xMin; xPos <= bounds.xMax; xPos++)
		{
			for (int yPos = bounds.yMin; yPos <= bounds.yMax; yPos++)
				tileMap.SetTile(new Vector3Int(xPos, yPos), tileSet.BackgroundTiles.PickRandom());
		}
	}
	public static void BackdropFill(Tilemap tileMap, RectInt bounds, ReadOnlySpan<TileBase> pickFrom)
	{
		for (int xPos = bounds.xMin; xPos <= bounds.xMax; xPos++)
		{
			for (int yPos = bounds.yMin; yPos <= bounds.yMax; yPos++)
				tileMap.SetTile(new Vector3Int(xPos, yPos), pickFrom.PickRandom());
		}
	}
}