using System;
using UnityEngine;

namespace TheRevenantsAge
{
    public class IdleWithoutState: StateMachineBehaviour
    {
        private CharacterAnimationController controller;
        //private bool isCrossedMiddle;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // isCrossedMiddle = false;
            controller ??= animator.GetComponent<CharacterAnimationController>();
            controller.SetAllAnimationToChosenWeapon();
            controller.ChangeChosenWeaponAnimator();
        }

        // public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     if (isCrossedMiddle) return;
        //     Debug.Log(stateInfo.normalizedTime);
        //     if (Math.Abs(stateInfo.normalizedTime - 0.15) < 0.01)
        //     {
        //         isCrossedMiddle = true;
        //         controller ??= animator.GetComponent<CharacterAnimationController>();
        //         controller.SetAllAnimationToChosenWeapon();
        //     }
        // }
    }
}