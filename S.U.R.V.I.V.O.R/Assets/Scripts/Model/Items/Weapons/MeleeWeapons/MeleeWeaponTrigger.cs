using System;
using System.Collections.Generic;
using System.Data;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace TheRevenantsAge
{
    public class MeleeWeaponTrigger: MonoBehaviour
    {
        [Min(0)] [SerializeField] private float damage;
        [Range(0, 1)] [SerializeField] private float penetrating;
        private Collider trigger;
        private IEntity owner;
        private HashSet<BodyPart> damagedBodyParts = new() ;

        public UnityEvent WhenAttacked;

        public IEntity Owner
        {
            get => owner;
            set
            {
                owner = value;
                if (owner is null)
                    SetActiveCollider(false);
            }
        }

        public float Damage
        {
            get => damage;
            set
            {
                if (value < 0)
                    throw new ConstraintException();
                damage = value;
            }
        }
        
        public float Penetrating
        {
            get => penetrating;
            set
            {
                if (value < 0)
                    throw new ConstraintException();
                damage = value;
            }
        }

        public float PenetratingDamage => Damage * Penetrating;
        public float NormalDamage => Damage - PenetratingDamage;
        
        private void Awake()
        {
            trigger = GetComponent<Collider>();
            SetActiveCollider(false);
            FindMyEntity();
        }

        public void SetActiveCollider(bool value)
        {
            trigger.enabled = value;
            trigger.isTrigger = value;
        }

        public void ClearDamagedBodyParts()
        {
            damagedBodyParts.Clear();
        }

        private void FindMyEntity()
        {
            var currentTransform = transform;
            while (currentTransform)
            {
                var entity = currentTransform.GetComponent<IEntity>();
                if (entity is not null)
                {
                    Owner = entity;
                    break;
                }

                currentTransform = currentTransform.parent;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            var pointerToBodyPart = other.GetComponent<PointerToBodyPart>();
            if (pointerToBodyPart == null 
                || pointerToBodyPart.BodyPart == null
                || pointerToBodyPart.MyEntity.Equals(Owner)
                || damagedBodyParts.Contains(pointerToBodyPart.BodyPart))
                return;
            
            damagedBodyParts.Add(pointerToBodyPart.BodyPart);
            var bodyPart = pointerToBodyPart.BodyPart;
            if (bodyPart is BodyPathWearableClothes wearableClothes)
            {
                var blockedDamage = Math.Min(NormalDamage, wearableClothes.CurrentArmor);
                wearableClothes.DamageArmor(blockedDamage);

                var damageToBody = Damage - blockedDamage;
                bodyPart.Hp -= damageToBody;
            }
            else
            {
                var damageToBody = Damage;
                bodyPart.Hp -= damageToBody;
            }
            
            WhenAttacked?.Invoke();
        }
    }
}