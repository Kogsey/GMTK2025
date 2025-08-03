using UnityEngine;

public class FloorItem : MonoBehaviour
{
	private ItemDrop drop;
	public SpriteRenderer spriteRenderer;
	public float timePerFrame = 0.1f;
	private AnimationHelper animationHelper;

	public void SetItem(ItemDrop itemDrop)
	{
		drop = itemDrop;
		animationHelper ??= new AnimationHelper(spriteRenderer);
		animationHelper.CheckedSwapToSequence(drop.Parent.Sprites, timePerFrame);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out PlayerController player))
		{
			player.AddStatEffector(drop);
			Destroy(gameObject);
		}
	}
}