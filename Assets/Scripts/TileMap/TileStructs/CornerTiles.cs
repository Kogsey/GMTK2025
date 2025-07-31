using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public struct CornerTiles
{
	[SerializeField] public TileBase TopLeft;
	[SerializeField] public TileBase TopRight;
	[SerializeField] public TileBase BottomLeft;
	[SerializeField] public TileBase BottomRight;
}
