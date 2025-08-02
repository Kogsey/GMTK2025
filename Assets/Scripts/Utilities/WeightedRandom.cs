using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeightedRandom<T>
{
	private readonly List<(float weight, T value)> _weights = new();

	public void AddElement(float weight, T value)
		=> _weights.Add((weight, value));

	public T Pick()
	{
		float totalWeights = 0f;

		foreach ((float weight, T value) in _weights)
			totalWeights += weight;

		float chosenWeight = Random.Range(0, totalWeights);

		foreach ((float weight, T value) in _weights)
		{
			if (chosenWeight < weight)
				return value;
			else
				chosenWeight -= weight;
		}

		return _weights[^1].value;
	}
}

public static class WeightedRandom
{
	public static T Pick<T>(IList<T> _weights, Func<T, float> weightFunc)
	{
		float totalWeights = 0f;

		foreach (T value in _weights)
			totalWeights += weightFunc(value);

		float chosenWeight = Random.Range(0, totalWeights);

		foreach (T value in _weights)
		{
			float weight = weightFunc(value);
			if (chosenWeight < weight)
				return value;
			else
				chosenWeight -= weight;
		}

		return _weights[^1];
	}
}