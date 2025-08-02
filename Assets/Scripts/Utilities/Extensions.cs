using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public static class Extensions
{
	// https://discussions.unity.com/t/normal-distribution-random/66530/2
	public static float RandomGaussianMinMax(float minValue = 0.0f, float maxValue = 1.0f)
	{
		float u, v, S;

		do
		{
			u = (2.0f * Random.value) - 1.0f;
			v = (2.0f * Random.value) - 1.0f;
			S = (u * u) + (v * v);
		}
		while (S >= 1.0f);

		// Standard Normal Distribution
		float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

		// Normal Distribution centred between the min and max value and clamped following the "three-sigma rule"
		float mean = (minValue + maxValue) / 2.0f;
		float sigma = (maxValue - mean) / 3.0f;
		return Mathf.Clamp((std * sigma) + mean, minValue, maxValue);
	}

	public static float RandomGaussianStdDev(float mean, float sigma)
	{
		float u, v, S;

		do
		{
			u = (2.0f * Random.value) - 1.0f;
			v = (2.0f * Random.value) - 1.0f;
			S = (u * u) + (v * v);
		}
		while (S >= 1.0f);

		// Standard Normal Distribution
		float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

		// Normal Distribution centred between the min and max value and clamped following the "three-sigma rule"
		return Mathf.Clamp((std * sigma) + mean, mean - (3 * sigma), mean + (3 * sigma));
	}

	public static T PickRandom<T>(this IList<T> list)
		=> list[Random.Range(0, list.Count)];

	public static T PickRandom<T>(this ReadOnlySpan<T> span)
		=> span[Random.Range(0, span.Length)];

	public static Vector2 RandomIn(Rect rect)
		=> new(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));

	public static Rect ZFlattened(this Bounds bounds)
		=> new((Vector2)bounds.min, (Vector2)bounds.size);

	public static RectInt Inflate(this RectInt rect, int amount)
		=> rect.Inflate(amount, amount);

	public static RectInt Inflate(this RectInt rect, int xAmount, int yAmount)
	{
		rect.x -= xAmount;
		rect.y -= yAmount;
		rect.width += xAmount * 2;
		rect.height += yAmount * 2;
		return rect;
	}

	public static Rect CellToWorld(this Tilemap tileMap, RectInt rectInt)
	{
		Rect rect = new()
		{
			min = tileMap.CellToWorld((Vector3Int)rectInt.min),
			max = tileMap.CellToWorld((Vector3Int)rectInt.max) + tileMap.cellSize,
		};
		return rect;
	}

	private const int DefaultDecayScale = 15;
	private static float InterpolationTime => Time.smoothDeltaTime;

	public static float SinLerp(float a, float b, float t)
	{
		float sinT = (Mathf.Sin((Mathf.PI * t) - (Mathf.PI / 2)) + 1) / 2;
		return Mathf.LerpUnclamped(a, b, sinT);
	}

	public static float SmoothInterpolate(float a, float b, float decayScale = DefaultDecayScale)
		=> b + ((a - b) * MathF.Exp(-decayScale * InterpolationTime));

	public static Vector2 SmoothInterpolate(Vector2 a, Vector2 b, float decayScale = DefaultDecayScale)
		=> b + ((a - b) * MathF.Exp(-decayScale * InterpolationTime));

	public static Vector3 SmoothInterpolate(Vector3 a, Vector3 b, float decayScale = DefaultDecayScale)
		=> b + ((a - b) * MathF.Exp(-decayScale * InterpolationTime));

	public static void GizmosDrawRect(Rect rect)
	{
		Span<Vector3> vectors = stackalloc Vector3[4];
		vectors[0] = new Vector3(rect.xMin, rect.yMin);
		vectors[1] = new Vector3(rect.xMin, rect.yMax);
		vectors[2] = new Vector3(rect.xMax, rect.yMax);
		vectors[3] = new Vector3(rect.xMax, rect.yMin);

		Gizmos.DrawLineStrip(vectors, true);
	}

	const float DefaultPPU = 16f;

	public static Vector3 PixelPerfectClamp(this Vector3 locationVector, float pixelsPerUnit = DefaultPPU)
	{
		Vector3 vectorInPixels = new(Mathf.CeilToInt(locationVector.x * pixelsPerUnit), Mathf.CeilToInt(locationVector.y * pixelsPerUnit), Mathf.CeilToInt(locationVector.z * pixelsPerUnit));
		return vectorInPixels / pixelsPerUnit;
	}

	public static Vector2 PixelPerfectClamp(this Vector2 locationVector, float pixelsPerUnit = DefaultPPU)
	{
		Vector2 vectorInPixels = new(Mathf.CeilToInt(locationVector.x * pixelsPerUnit), Mathf.CeilToInt(locationVector.y * pixelsPerUnit));
		return vectorInPixels / pixelsPerUnit;
	}
}