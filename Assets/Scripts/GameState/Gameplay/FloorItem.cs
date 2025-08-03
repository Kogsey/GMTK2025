using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FloorItem : MonoBehaviour
{
	private ItemDrop drop;
	public SpriteRenderer spriteRenderer;
	public float timePerFrame = 0.1f;
	private AnimationHelper animationHelper;

	public float FadeTime = 1f;
	public float Timer;
	public bool Obtained = false;

	Light2D BackLight;

	private void Awake()
		=> BackLight = GetComponentInChildren<Light2D>();

	public void SetItem(ItemDrop itemDrop)
	{
		drop = itemDrop;
		animationHelper ??= new AnimationHelper(spriteRenderer);
		animationHelper.CheckedSwapToSequence(drop.Parent.Sprites, timePerFrame);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!Obtained && collision.TryGetComponent(out PlayerController player))
		{
			Obtained = true;
			player.AddStatEffector(drop);
		}
	}

	private void Update()
	{
		animationHelper.Update();

		if (Obtained)
		{
			Timer += Time.deltaTime;

			if (Timer > FadeTime)
				Destroy(gameObject);

			Color lerpOut = Color.Lerp(Color.white, new Color(0, 0, 0, 0), Timer / FadeTime);
			spriteRenderer.color = lerpOut;
			if (BackLight != null)
				BackLight.color = lerpOut;
		}
	}
}