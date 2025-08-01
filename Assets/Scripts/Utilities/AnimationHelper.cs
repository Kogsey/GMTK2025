using UnityEngine;

public class AnimationHelper
{
	public enum FrameType { Static, Sequence }

	public bool FreezeChanges { get; set; }
	public bool LoopAnimation { get; set; } = true;
	public bool Paused { get; set; }
	public bool AnimationEnded { get; private set; }
	public int currentFrame { get; private set; }

	private readonly SpriteRenderer spriteRenderer;
	private FrameType currentFrameType;
	private float frameTimer;
	private FrameData[] currentFrames;

	public AnimationHelper(SpriteRenderer spriteRenderer)
		=> this.spriteRenderer = spriteRenderer;

	public void CheckedSwapToSequence(FrameData[] loop)
	{
		if ((currentFrameType != FrameType.Sequence || currentFrames != loop) && !FreezeChanges)
			ForceToLoop(loop);
	}

	public void ForceToLoop(FrameData[] loop)
	{
		EndLastAnimation();
		currentFrameType = FrameType.Sequence;
		currentFrames = loop;
		SetCurrentSprite(loop[0].Frame);
	}

	public void CheckedSwapToFrame(Sprite sprite)
	{
		if (!FreezeChanges)
			ForceToFrame(sprite);
	}

	public void ForceToFrame(Sprite frame)
	{
		EndLastAnimation();
		currentFrameType = FrameType.Static;
		SetCurrentSprite(frame);
	}

	private void EndLastAnimation()
	{
		currentFrame = 0;
		frameTimer = 0;
		AnimationEnded = false;
		currentFrames = null;
	}

	public void Update()
	{
		if (currentFrameType == FrameType.Sequence)
		{
			if (!Paused)
				frameTimer += Time.deltaTime;
			if (frameTimer > currentFrames[currentFrame].FrameTime)
			{
				currentFrame++;

				if (currentFrame >= currentFrames.Length)
				{
					if (LoopAnimation)
					{
						currentFrame = 0;
					}
					else
					{
						currentFrame = currentFrames.Length - 1;
						AnimationEnded = true;
					}
				}

				SetCurrentSprite(currentFrames[currentFrame].Frame);
				frameTimer = 0;
			}
		}
	}

	private void SetCurrentSprite(Sprite sprite)
		=> spriteRenderer.sprite = sprite;
}