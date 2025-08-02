using UnityEngine;

public class ParallaxBGManager : MonoBehaviour
{
	public SpriteRenderer This;
	public SpriteRenderer Left;
	public SpriteRenderer Right;

	public float PPU = 16f;

	public float yOffset;
	public float parallaxFactor;
	public Transform Tracking;

	private float length;
	public Vector2 startingPosition;

	void Update()
	{
		if (Tracking != null)
		{
			float temp = Tracking.position.x * (1 - parallaxFactor);
			float distance = Tracking.position.x * parallaxFactor;

			Vector3 newPosition = new(startingPosition.x + distance, startingPosition.y + yOffset, 0);

			transform.position = newPosition.PixelPerfectCeil(PPU);

			if (temp > startingPosition.x + (length / 2))
				startingPosition.x += length;
			else if (temp < startingPosition.x - (length / 2))
				startingPosition.x -= length;
		}
	}

	public void Setup(Sprite sprite, Transform tracking, int layer, float factor, Vector2 startingPos)
	{
		This.sprite = sprite;
		Left.sprite = sprite;
		Right.sprite = sprite;

		int sorting = layer + 2;
		This.sortingOrder = sorting;
		Left.sortingOrder = sorting;
		Right.sortingOrder = sorting;

		length = This.bounds.size.x;

		Tracking = tracking;
		parallaxFactor = factor * layer;
		startingPosition = startingPos;
	}
}