using UnityEngine;

namespace TheRevenantsAge
{
    public class TakeWeaponState : StateMachineBehaviour
    {
        private CharacterAnimationController controller;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            controller ??= animator.GetComponent<CharacterAnimationController>();
            controller.SetAllAnimationToChosenWeapon();
        }
    }
}