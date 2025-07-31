using System;
using UnityEditor.Toolbars;
using UnityEngine;

public class DevGenMenu : MonoBehaviour
{
	public TileMapGenerator TileMapGen;

	[Range(1, 20)]
	public int Level = 1;

	public RangeInt ConnectionGroundOffsetRange;
	public RangeInt ConnectionHallHeightRange;
	public RangeInt ConnectionLengthRange;

	public bool RegenMap = true;

	public void Update()
	{
		if (RegenMap)
		{

			TileMapGen.GenerateTileMap(Level);
			RegenMap = false;
		}
	}
}