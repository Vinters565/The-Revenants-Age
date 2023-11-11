using System;
using UnityEngine;

namespace TheRevenantsAge
{
    public static class AnimatorExtension
    {
        public static void SetAnimations(this Animator animator,
            AnimatorOverrideController overrideController)
        {
            animator.runtimeAnimatorController = overrideController;
        }

        public static void ToDefaultAnimations(this Animator animator)
        {
            var overrideAnimations = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = overrideAnimations;
        }
    }
}