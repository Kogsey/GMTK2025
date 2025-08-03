using System.Collections.Generic;
using UnityEngine;

public class RandomVariants : MonoBehaviour
{
	public Animator Animator;
	public AnimationClip[] Variants;

	private void Awake()
	{
		AnimationClip picked = Variants.PickRandom();
		AnimatorOverrideController aoc = new(Animator.runtimeAnimatorController);
		List<KeyValuePair<AnimationClip, AnimationClip>> animations = new();
		foreach (AnimationClip a in aoc.animationClips)
			animations.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, picked));
		aoc.ApplyOverrides(animations);
		Animator.runtimeAnimatorController = aoc;
	}
}