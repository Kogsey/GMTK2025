using System;
using UnityEngine;

public class BackdropManager : MonoBehaviour
{
	public Transform ParallaxTracking;

	public SpriteRenderer BackdropPrefab;
	public ParallaxBGManager ParallaxPrefab;

	public Sprite[] Backdrops;
	private SpriteRenderer[] backDropChildren;

	public Sprite[] ParallaxBackdrops;
	public Vector2 ParallaxPosition = Vector2.zero;
	public float ParallaxFactor = 1.0f;

	[Range(0f, 3f)]
	public float Distance = 0f;

	private void Start()
	{
		if (Backdrops != null)
		{
			backDropChildren = new SpriteRenderer[Backdrops.Length];

			for (int i = 0; i < Backdrops.Length; i++)
			{
				backDropChildren[i] = Instantiate(BackdropPrefab, transform);
				backDropChildren[i].sprite = Backdrops[i];
			}
		}

		if (ParallaxBackdrops != null)
		{
			for (int i = 0; i < ParallaxBackdrops.Length; i++)
			{
				ParallaxBGManager parallaxBG = Instantiate(ParallaxPrefab);
				parallaxBG.Setup(ParallaxBackdrops[i], ParallaxTracking, i + 1, ParallaxFactor, ParallaxPosition);
			}
		}
	}

	private void Update()
	{
		if (backDropChildren != null)
		{
			for (int i = 0; i < backDropChildren.Length; i++)
			{
				float thisT = Mathf.Clamp(Distance - i, -1f, 1f);
				float opacity = Extensions.SinLerp(1f, 0f, thisT);

				if (opacity > 0f)
				{
					backDropChildren[i].enabled = true;
					backDropChildren[i].color = new Color(1, 1, 1, opacity);
				}
				else
				{
					backDropChildren[i].enabled = false;
				}
			}
		}
	}
}