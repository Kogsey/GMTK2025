using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

[Serializable]
public struct LampInfo
{
	public Color LampColour;
	public Sprite[] LampFrames;
}

public class LightHelper : MonoBehaviour
{
	public Light2D Light;
	public SpriteRenderer SpriteRenderer;
	public SpriteRenderer Chain;
	private AnimationHelper animationHelper;
	public float TimePerFrame;
	public LampInfo LampType1;
	public LampInfo LampType2;
	public LampInfo LampType3;
	public LampInfo LampType4;
	private const int lampTypes = 4;

	public int minLayerDepth = -10;
	public int maxLayerDepth = 10;

	public LampInfo GetLampType(int index)
		=> index switch
		{
			0 => LampType1,
			1 => LampType2,
			2 => LampType3,
			3 => LampType4,
			_ => throw new IndexOutOfRangeException(),
		};

	private void Start()
	{
		animationHelper = new(SpriteRenderer);
		RandGenLamp();
	}

	private void Update()
		=> animationHelper.Update();

	private void RandGenLamp()
	{
		LampInfo lamp = GetLampType(Random.Range(0, lampTypes));

		int sorting = Random.Range(minLayerDepth, maxLayerDepth + 1);
		SpriteRenderer.sortingOrder = sorting;
		Chain.sortingOrder = sorting;

		Light.color = lamp.LampColour;

		animationHelper.CheckedSwapToSequence(lamp.LampFrames, TimePerFrame);
	}

	public void SetChainHeight(float height)
	{
		Chain.transform.localPosition = (1 + (height / 2)) * Vector3.up;
		Chain.size = new(Chain.size.x, height);
	}
}