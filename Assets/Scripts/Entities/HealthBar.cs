using System;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
	private const float RefScale = 1f;
	private const float BorderPadding = 0.1f;
	private float maxWidth;
	public float scale;
	public SpriteRenderer Border;
	public SpriteRenderer Bar;
	public SpriteRenderer Block;
	public SpriteRenderer Absorption;

	private void Update()
	{
		if (transform.parent != null)
		{
			int scaleDir = transform.parent.localScale.x < 0 ? -1 : 1;
			transform.localScale = new Vector3(scale * scaleDir, scale, 1);
		}
	}

	public void UpdateAll(int maxHealth, int health)
	{
		UpdateMaxHealth(maxHealth);
		UpdateHealth(health, 0, 0);
	}

	public void UpdateMaxHealth(int maxHealth)
	{
		maxWidth = maxHealth / RefScale;
		Border.size = new Vector2(maxWidth + BorderPadding, Border.size.y);
	}

	public void UpdateHealth(int health, int block = 0, int absorption = 0)
	{
		float bar = MathF.Max(health / RefScale, 0.5f);
		Bar.size = new Vector2(bar, 1);
		Bar.transform.localPosition = new Vector3((Bar.size.x - maxWidth) / 2, 0, 0);

		Absorption.size = new Vector2(Bar.size.x + (absorption / RefScale), 1);
		Absorption.transform.localPosition = new Vector3((Absorption.size.x - maxWidth) / 2, 0, 0);

		Block.size = new Vector2(Absorption.size.x + (block / RefScale), 1);
		Block.transform.localPosition = new Vector3((Block.size.x - maxWidth) / 2, 0, 0);
	}
}