using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    [RequireComponent(typeof(BaseEntity))]
    [RequireComponent(typeof(AnimationsStateController))]
    public class BaseFightEntity: MonoBehaviour, IFightEntity
    {
        private BaseEntity baseEntity;
        [SerializeField] [Min(1)] private int speedInFightScene = 1;
        [SerializeField] [Min(1)] private float initiative;
        [SerializeField] private List<MeleeWeaponTrigger> meleeAttackTriggers;
        private AnimationsStateController animationsStateController;
        
        public Body Body => baseEntity.Body;
        public virtual Aimer Aimer => null;
        public event Action<float, Vector3> WhenDamagedOrHealed;
        public event Action<float, Vector3> WhenDamagedOrRecoveryArmor;

        protected virtual void Awake()
        {
            baseEntity = GetComponent<BaseEntity>();
            animationsStateController = GetComponent<AnimationsStateController>();
        }

        protected virtual void OnEnable()
        {
            baseEntity.Body.WhenDamagedOrHealed += BodyOnWhenDamagedOrHealed;
            if (baseEntity.Body is IWearClothes wearClothesBody)
                wearClothesBody.WhenDamagedOrRecoveryArmor += WearClothesBodyOnWhenDamagedOrRecoveryArmor;
        }
        protected virtual void OnDisable()
        {
            baseEntity.Body.WhenDamagedOrHealed -= BodyOnWhenDamagedOrHealed;
            if (baseEntity.Body is IWearClothes wearClothesBody)
                wearClothesBody.WhenDamagedOrRecoveryArmor -= WearClothesBodyOnWhenDamagedOrRecoveryArmor;
            
        }

        public float Initiative
        {
            get => initiative;
            set => initiative = value;
        }

        public int SpeedInFightScene
        {
            get => speedInFightScene;
            set => speedInFightScene = value;
        }
        
        public virtual void Attack(Vector3 targetPoint, AttackType type)
        {
            animationsStateController.TriggerAttack();
        }

        public void UpDamageToMeleeAttacks(float added)
        {
            foreach (var meleeAttackTrigger in meleeAttackTriggers)
                meleeAttackTrigger.Damage += added;
        }
        
        private void WearClothesBodyOnWhenDamagedOrRecoveryArmor(float diff, Vector3 pos)
        {
            WhenDamagedOrRecoveryArmor?.Invoke(diff, pos);
        }

        private void BodyOnWhenDamagedOrHealed(float diff, Vector3 pos)
        {
            WhenDamagedOrHealed?.Invoke(diff, pos);
        }
        
        public void ANIMATION_EVENT_ActivateMeleeTriggers()
        {
            if (baseEntity is IEntityWithWeapon entityWithWeapon && entityWithWeapon.ChosenWeapon != null)
            {
                entityWithWeapon.ChosenWeapon.IsActive = true;
            }
            else
            {
                foreach (var trigger in meleeAttackTriggers)
                {
                    trigger.SetActiveCollider(true);
                }   
            }
        }
        
        public void ANIMATION_EVENT_DeactivateMeleeTriggers()
        {
            if (baseEntity is IEntityWithWeapon entityWithWeapon && entityWithWeapon.ChosenWeapon != null)
            {
                entityWithWeapon.ChosenWeapon.IsActive = false;
            }
            else
            {
                foreach (var trigger in meleeAttackTriggers)
                {
                    trigger.SetActiveCollider(false);
                    trigger.ClearDamagedBodyParts();
                }   
            }
        }
    }
}