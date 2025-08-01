using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SwordController : MonoBehaviour
{
	private AnimationHelper animationHelper;
	private PlayerController playerController;
	private Aggressor aggressor;
	public Collider2D SmallSwingCollider;
	public Collider2D BigSwingCollider;

	private Collider2D CurrentCollider
		=> isPrimary ? SmallSwingCollider : BigSwingCollider;

	private int CurrentHitFrame
		=> isPrimary ? SmallSwingHitboxFrame : BigSwingHitboxFrame;

	private bool isSwung;
	private bool isPrimary;

	public HitInfo BigHitInfo;
	public HitInfo SmallHitInfo;

	public FrameData[] Idle;
	public int BigSwingHitboxFrame;
	public FrameData[] Swing;
	public FrameData[] BigSwing;
	public int SmallSwingHitboxFrame;

	// Start is called before the first frame update
	void Start()
	{
		aggressor = GetComponent<Aggressor>();
		animationHelper = new(GetComponent<SpriteRenderer>())
		{
			LoopAnimation = false,
		};
		playerController = GetComponentInParent<PlayerController>();
		ResetAttackVariables();
	}

	// Update is called once per frame
	void Update()
	{
		animationHelper.Update();

		if (!isSwung)
		{
			if (Input.GetKey(Settings.CurrentSettings.Primary))
				PrimaryAttack();
			else if (Input.GetKey(Settings.CurrentSettings.Secondary))
				SecondaryAttack();
		}
		else
		{
			if (animationHelper.currentFrame >= CurrentHitFrame)
			{
				CurrentCollider.enabled = true;
			}

			if (animationHelper.AnimationEnded)
				ResetAttackVariables();
		}
	}

	private void PrimaryAttack()
	{
		isSwung = isPrimary = true;
		animationHelper.CheckedSwapToSequence(Swing);
		animationHelper.LoopAnimation = false;
		aggressor.HitInfo = SmallHitInfo;
	}

	private void SecondaryAttack()
	{
		isSwung = true;
		isPrimary = false;
		animationHelper.CheckedSwapToSequence(BigSwing);
		animationHelper.LoopAnimation = false;
		aggressor.HitInfo = BigHitInfo;
	}

	private void ResetAttackVariables()
	{
		isSwung = isPrimary = false;
		SmallSwingCollider.enabled = false;
		BigSwingCollider.enabled = false;
		animationHelper.CheckedSwapToSequence(Idle);
		animationHelper.LoopAnimation = true;
		aggressor.HitInfo = HitInfo.GetImpotent();
	}

	public void DeathClatter()
		=> Destroy(gameObject);

	/*
	animationHelper.ForceToLoop(Idle);

	animationHelper.LoopAnimation = false;
	animationHelper.FreezeChanges = true;

	collider2D.isTrigger = false;
	transform.parent = null;
	*/
}