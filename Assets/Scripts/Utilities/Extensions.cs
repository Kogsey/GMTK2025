using System;
using System.Collections.Generic;
using UnityEngine;
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

		// Normal Distribution centred between the min and max value
		// and clamped following the "three-sigma rule"
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

		// Normal Distribution centred between the min and max value
		// and clamped following the "three-sigma rule"
		return Mathf.Clamp((std * sigma) + mean, mean - (3 * sigma), mean + (3 * sigma));
	}

	public static T PickRandom<T>(this IList<T> list)
		=> list[Random.Range(0, list.Count)];

	public static T PickRandom<T>(this ReadOnlySpan<T> span)
		=> span[Random.Range(0, span.Length)];

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
}