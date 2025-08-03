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

	public float CooldownCounter = 0f;
	public float Cooldown = 2f;
	public float SwingSpeedMultiplier = 1f;
	public float DamageMultiplier = 1f;
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
		CooldownCounter += Time.deltaTime;

		if (!isSwung)
		{
			if (Input.GetKey(Settings.CurrentSettings.Primary))
				PrimaryAttack();
			else if (CooldownCounter > Cooldown && Input.GetKey(Settings.CurrentSettings.Secondary))
				SecondaryAttack();
		}
		else
		{
			if (animationHelper.CurrentFrame >= CurrentHitFrame)
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
		animationHelper.AnimationSpeed = SwingSpeedMultiplier;
		animationHelper.LoopAnimation = false;
		aggressor.HitInfo = SmallHitInfo.WithMultiplier(DamageMultiplier);
	}

	private void SecondaryAttack()
	{
		isSwung = true;
		isPrimary = false;
		animationHelper.CheckedSwapToSequence(BigSwing);
		animationHelper.AnimationSpeed = SwingSpeedMultiplier;
		animationHelper.LoopAnimation = false;
		aggressor.HitInfo = BigHitInfo.WithMultiplier(DamageMultiplier);
	}

	private void ResetAttackVariables()
	{
		isSwung = isPrimary = false;
		SmallSwingCollider.enabled = false;
		BigSwingCollider.enabled = false;
		animationHelper.CheckedSwapToSequence(Idle);
		animationHelper.AnimationSpeed = 1f;
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