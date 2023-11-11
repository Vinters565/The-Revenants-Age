using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

namespace TheRevenantsAge
{
    public class CharacterAnimationController : AnimationsStateController
    {
        private static readonly int IsRemoveWeaponKey = Animator.StringToHash("isRemoveWeapon");
        private static readonly int SetWeaponKey = Animator.StringToHash("SetWeapon");
        private static readonly int SetLowMeleeAttack = Animator.StringToHash("LowMeleeAttack");
        private static readonly int SetHighMeleeAttack = Animator.StringToHash("HighMeleeAttack");
        private static readonly int ReloadKey = Animator.StringToHash("Reload");

        public const string TAKE_WEAPON_BASE_ANIMATION_NAME = "BaseTakeWeapon";
        public const string IDLE_BASE_ANIMATION_NAME = "BaseIdle";
        public const string IDLE_WITHOUT_WEAPON_BASE_ANIMATION_NAME = "BaseIdleWithoutWeapon";
        public const string REMOVE_WEAPON_BASE_ANIMATION_NAME = "BaseRemoveWeapon";

        private IFightCharacter character;

        [SerializeField] private Transform primaryGunContainerOffset;
        [SerializeField] private Transform secondaryGunContainerOffset;
        [SerializeField] private Transform meleeWeaponContainerOffset;

        private Dictionary<WeaponAnimator, bool> chosenWeaponAnimators = new ();
        private Dictionary<WeaponAnimator, MultiParentConstraint> weaponAnimatorMultiParentConstraint = new();
        
        public bool IsRemoveWeapon => animator.GetBool(IsRemoveWeaponKey);
        public bool IsHaveWeapon => animator.GetBool(IsHaveWeaponKey);
        public bool IsAiming => animator.GetBool(IsAimingKey);


        public bool IsCanSetWeapon => IsIdleState;
        public bool IsIdleState => animator
            .GetCurrentAnimatorStateInfo(0)
            .IsName("Idle");
        
        private IEnumerable<WeaponAnimator> GetAnimators()
        {
            return chosenWeaponAnimators
                .Where(x => !x.Value)
                .Select(x => x.Key);
        }

        
        protected override void Awake()
        {
            base.Awake();
            character = GetComponent<IFightCharacter>();
        }

        private void OnEnable()
        {
            if(character == null) return;
            character.ChosenWeaponChanged += OnChosenWeaponChanged;
            character.RestoreEnd += CharacterOnRestoreEnd;
        }

        private void OnDisable()
        {
            if(character == null) return;
            character.ChosenWeaponChanged -= OnChosenWeaponChanged;
            character.RestoreEnd -= CharacterOnRestoreEnd;
        }
        
        private void LateUpdate()
        {
            ChangeOffsetToDefaultButIgnoreChosenWeapon();
            //ChangeConstraints();
        }

        public void CharacterOnRestoreEnd()
        {
            var primary = character?.PrimaryGun?.GetComponent<WeaponAnimator>();
            if (primary != null)
            {
                chosenWeaponAnimators[primary] = false;
                weaponAnimatorMultiParentConstraint[primary] = GetConstraint(primary);
            }


            var secondary = character?.SecondaryGun?.GetComponent<WeaponAnimator>();
            if (secondary != null)
            {
                chosenWeaponAnimators[secondary] = false;
                weaponAnimatorMultiParentConstraint[secondary] = GetConstraint(secondary);
            }

            var melee = character?.MeleeWeapon?.GetComponent<WeaponAnimator>();
            if (melee != null)
            {
                chosenWeaponAnimators[melee] = false;
                weaponAnimatorMultiParentConstraint[melee] = GetConstraint(melee);
            }
        }

        public void LowMeleeAttack() => animator.SetTrigger(SetLowMeleeAttack);
        public void HighMeleeAttack() => animator.SetTrigger(SetHighMeleeAttack);

        public void Reload() => animator.SetTrigger(ReloadKey);
        
        private void ChangeConstraints()
        {
            foreach (var weaponAnimator in GetAnimators())
                ChangeConstraintToDefault(weaponAnimatorMultiParentConstraint[weaponAnimator]);
        }

        private void ChangeOffsetToDefaultButIgnoreChosenWeapon()
        {
            foreach (var weaponAnimator in GetAnimators())
                weaponAnimator.ChangeWeaponOffsetToDefault();
        }
        
        public void EquipAllWeapons()
        {
            var primary = character.PrimaryGun;
            var secondary = character.SecondaryGun;
            var melee = character.MeleeWeapon;
            
            if (primary != null)
            {
                primary.transform.SetParent(primaryGunContainerOffset, false);
                primary.Owner = character;
            }
            
            if (secondary != null)
            {
                secondary.transform.SetParent(secondaryGunContainerOffset, false);
                secondary.Owner = character;
            }
            
            if (melee != null)
            {
                melee.transform.SetParent(meleeWeaponContainerOffset, false);
                melee.Owner = character;
            }
        }

        public void SetAllAnimationToChosenWeapon()
        {
            try
            {
                var newAnimations = character.ChosenWeapon.GetComponent<WeaponAnimator>().Animations;
                animator.SetAnimations(newAnimations);
                Debug.Log("NewAnimations by: " + newAnimations.name);
            }
            catch (Exception)
            {
                animator.ToDefaultAnimations();
                Debug.Log("DefaultAnimations");
            }
        }
        
        private void OnChosenWeaponChanged(Weapon oldWeapon, Weapon newWeapon)
        {
            var isRemoveWeapon = newWeapon == null;
            var isHaveWeapon = oldWeapon != null;
            if (isRemoveWeapon && !isHaveWeapon)
                return;

            animator.SetBool(IsHaveWeaponKey, isHaveWeapon);
            animator.SetBool(IsRemoveWeaponKey, isRemoveWeapon);
            animator.SetTrigger(SetWeaponKey);

            //Меняем анимации
            var overrideAnimations = new AnimatorOverrideController(animator.runtimeAnimatorController);
            if (isHaveWeapon)
            {
                var oldAnimations = oldWeapon.GetComponent<WeaponAnimator>().Animations;
                overrideAnimations[IDLE_BASE_ANIMATION_NAME] = oldAnimations[IDLE_BASE_ANIMATION_NAME];
                overrideAnimations[REMOVE_WEAPON_BASE_ANIMATION_NAME] =
                    oldAnimations[REMOVE_WEAPON_BASE_ANIMATION_NAME];

                if (isRemoveWeapon)
                {
                    animator.SetAnimations(overrideAnimations);
                    return;
                }
            }

            var newWeaponAnimator = newWeapon.GetComponent<WeaponAnimator>();
            var newWeaponAnimations = newWeaponAnimator.Animations;
            overrideAnimations[IDLE_WITHOUT_WEAPON_BASE_ANIMATION_NAME] =
                newWeaponAnimations[IDLE_WITHOUT_WEAPON_BASE_ANIMATION_NAME];
            overrideAnimations[TAKE_WEAPON_BASE_ANIMATION_NAME] = newWeaponAnimations[TAKE_WEAPON_BASE_ANIMATION_NAME];
            newWeaponAnimator.OnTake();
            animator.SetAnimations(overrideAnimations);
        }

        public void ChangeChosenWeaponAnimator()
        {
            var chosenWeaponAnimator = character.ChosenWeapon?.GetComponent<WeaponAnimator>();
            
            foreach (var weaponAnimator in chosenWeaponAnimators.Keys.ToArray())
                chosenWeaponAnimators[weaponAnimator] = false;
            if (chosenWeaponAnimator != null)
                chosenWeaponAnimators[chosenWeaponAnimator] = true;
        }


        private MultiParentConstraint GetConstraint(WeaponAnimator weaponAnimator)
        {
            var currentTransform = weaponAnimator.transform;
            while (currentTransform)
            {
                var result = currentTransform.GetComponent<MultiParentConstraint>();
                if (result != null)
                    return result;
                currentTransform = currentTransform.parent;
            }

            throw new InvalidOperationException();
        }

        private void ChangeConstraintToDefault(MultiParentConstraint constraint)
        {
            var sourceObjects = constraint.data.sourceObjects;
            sourceObjects.SetWeight(0, 1);
            for (var i = 1; i < sourceObjects.Count; i++)
                sourceObjects.SetWeight(i, 0);

            constraint.data.sourceObjects = sourceObjects;
        }
    }
}