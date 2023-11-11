using System;
using System.Collections.Generic;
using System.Linq;
using Extension;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public class MeleeWeapon : Weapon
    {
        [SerializeField] protected MeleeWeaponData data;
        private List<MeleeWeaponTrigger> triggers;
        private Vector3 targetPoint;
        private CharacterSkills skills; //TODO: добавить зависимось от скилов
        private bool isActive;
        private Aimer aimer;

        public MeleeWeaponData Data => data;

        public override bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                foreach (var trigger in triggers)
                {
                    trigger.SetActiveCollider(isActive);
                    if(!isActive)
                        trigger.ClearDamagedBodyParts();
                }
            }
        }

        public override HandlingTypes HandlingType
        {
            get
            {
                return data.HandlingType switch
                {
                    MeleeWeaponData.HandlingTypes.Long => HandlingTypes.LongMelee,
                    MeleeWeaponData.HandlingTypes.Short => HandlingTypes.ShortMelee,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        public override Aimer Aimer => aimer;
        public override float Ergonomics => data.Ergonomics;

        protected override void Awake()
        {
            base.Awake();
            triggers = transform
                .IterateByAllChildren()
                .Select(x => x.GetComponent<MeleeWeaponTrigger>())
                .Where(x => x != null)
                .ToList();
            aimer = GetComponent<MeleeWeaponAimer>();
        }

        public override void Attack(Vector3 targetPoint, CharacterSkills skills)
        {
            this.targetPoint = targetPoint;
            this.skills = skills;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            baseItem.ItemOwnerChanged.AddListener(OnItemOwnerChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            baseItem.ItemOwnerChanged.RemoveListener(OnItemOwnerChanged);
        }

        private void OnItemOwnerChanged(ICharacter newOwner)
        {
            foreach (var trigger in triggers)
                trigger.Owner = newOwner;
        }
    }
}