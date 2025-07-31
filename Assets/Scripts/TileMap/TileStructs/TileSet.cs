using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public struct TileSet
{
	public TileBase[] BackgroundTiles;

	public readonly EdgeTiles AirEdges => default;

	[SerializeField] public EdgeTiles EdgeTiles;
	[SerializeField] public CornerTiles InnerCorners;
	[SerializeField] public CornerTiles OuterCorners;
}
