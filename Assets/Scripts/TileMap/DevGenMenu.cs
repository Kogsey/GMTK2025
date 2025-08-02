using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DevGenMenu : MonoBehaviour
{
	public TileMapGenerator TileMapGen;

	[Range(1, 20)]
	public int Level = 1;

	public RangeInt ConnectionGroundOffsetRange;
	public RangeInt ConnectionHallHeightRange;
	public RangeInt ConnectionLengthRange;

	public bool RegenMap = true;

	public bool ApplyLighting;

	public float LightingIntensity = 1f;
	private float _currentLightIntensity;

	public float LightingOuterRadius = 10f;
	private float _currentLightingOuterRadius;

	public float LightingInnerRadius = 10f;
	private float _currentLightingInnerRadius;

	[Range(0f, 1f)]
	public float LightingFallOff = 0.5f;

	private float _currentLightFallOff;

	private bool CheckLightingChanged()
	{
		if (ApplyLighting
			&& (_currentLightIntensity != LightingIntensity
			|| _currentLightingOuterRadius != LightingOuterRadius
			|| _currentLightingInnerRadius != LightingInnerRadius
			|| _currentLightFallOff != LightingFallOff))
		{
			_currentLightIntensity = LightingIntensity;
			_currentLightingOuterRadius = LightingOuterRadius;
			_currentLightingInnerRadius = LightingInnerRadius;
			_currentLightFallOff = LightingFallOff;

			return true;
		}

		return false;
	}

	public void Update()
	{
		if (RegenMap)
		{
			TileMapGen.GenerateTileMap(Level);
			RegenMap = false;
		}

		if (CheckLightingChanged())
		{
			Light2D[] lights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
			foreach (Light2D light in lights)
			{
				light.intensity = _currentLightIntensity;
				light.falloffIntensity = _currentLightFallOff;
				light.pointLightOuterRadius = _currentLightingOuterRadius;
				light.pointLightInnerRadius = _currentLightingInnerRadius;
			}
		}
	}
}