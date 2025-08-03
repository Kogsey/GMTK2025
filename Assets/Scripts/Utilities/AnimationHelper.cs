using UnityEngine;

public class AnimationHelper
{
	public enum FrameType { Static, Sequence }

	public bool FreezeChanges { get; set; }
	public bool LoopAnimation { get; set; } = true;
	public bool Paused { get; set; }
	public bool AnimationEnded { get; private set; }
	public int CurrentFrame { get; private set; }
	public float AnimationSpeed { get; set; } = 1f;

	private readonly SpriteRenderer spriteRenderer;
	private FrameType currentFrameType;
	private float frameTimer;
	private FrameData[] currentFrames;

	public AnimationHelper(SpriteRenderer spriteRenderer)
		=> this.spriteRenderer = spriteRenderer;

	public void CheckedSwapToSequence(Sprite[] sLoop, float timePerFrame)
	{
		FrameData[] loop = new FrameData[sLoop.Length];
		for (int i = 0; i < sLoop.Length; i++)
		{
			loop[i] = new()
			{
				Frame = sLoop[i],
				FrameTime = timePerFrame,
			};
		}

		if ((currentFrameType != FrameType.Sequence || currentFrames != loop) && !FreezeChanges)
			ForceToLoop(loop);
	}

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
		CurrentFrame = 0;
		frameTimer = 0;
		AnimationEnded = false;
		currentFrames = null;
	}

	public void Update(float deltaTime)
	{
		if (currentFrameType == FrameType.Sequence)
		{
			if (!Paused)
				frameTimer += deltaTime * AnimationSpeed;
			if (frameTimer > currentFrames[CurrentFrame].FrameTime)
			{
				CurrentFrame++;

				if (CurrentFrame >= currentFrames.Length)
				{
					if (LoopAnimation)
					{
						CurrentFrame = 0;
					}
					else
					{
						CurrentFrame = currentFrames.Length - 1;
						AnimationEnded = true;
					}
				}

				SetCurrentSprite(currentFrames[CurrentFrame].Frame);
				frameTimer = 0;
			}
		}
	}

	public void Update()
		=> Update(Time.smoothDeltaTime);

	private void SetCurrentSprite(Sprite sprite)
		=> spriteRenderer.sprite = sprite;
}