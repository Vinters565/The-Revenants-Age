using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public class AnimationsStateController : MonoBehaviour
    {
        public static readonly int IsAimingKey = Animator.StringToHash("isAiming");
        public static readonly int IsHaveWeaponKey = Animator.StringToHash("isHaveWeapon");
        public static readonly int EnterAttackKey = Animator.StringToHash("EnterAttack");
        public static readonly int IsRunKey = Animator.StringToHash("isRun");

        protected Animator animator;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void AimingAnimationOn()
        {
            animator.SetBool(IsAimingKey, true);
        }

        public void AimingAnimationOff()
        {
            animator.SetBool(IsAimingKey, false);
        }

        public void RunAnimationOn()
        {
            animator.SetBool(IsRunKey, true);
        }
        
        public void RunAnimationOff()
        {
            animator.SetBool(IsRunKey, false);
        }

        public void DisableAnimator()
        {
            animator.enabled = false;
        }

        public void TriggerAttack() => animator.SetTrigger(EnterAttackKey);
    }
}
