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

	[Range(0f, 1f)]
	public float BackGroundLerp = 0f;

	private void Start()
	{
		if (Backdrops != null)
		{
			backDropChildren = new SpriteRenderer[Backdrops.Length];

			for (int i = 0; i < Backdrops.Length; i++)
			{
				backDropChildren[i] = Instantiate(BackdropPrefab, transform);
				backDropChildren[i].sprite = Backdrops[i];
				backDropChildren[i].sortingOrder = i - 4;
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

	public bool AutoBackdrop = true;

	private void Update()
	{
		if (AutoBackdrop)
			BackGroundLerp = BetterSingleton<GameplayLoop>.Instance.Level / 10f;

		if (backDropChildren != null)
		{
			for (int i = 1; i < Backdrops.Length; i++)
			{
				float thisT = Mathf.Clamp01((BackGroundLerp * backDropChildren.Length) - (i - 1));
				float opacity = Extensions.SinLerp(0f, 1f, thisT);

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