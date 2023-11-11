using System;
using System.Collections.Generic;
using Inventory.SpecialCells;
using UnityEngine;

namespace TheRevenantsAge
{
    public class BaseGlobalMapCharacter: BaseGlobalMapEntity, IGlobalMapCharacter, ITurnEndAction
    {
        [SerializeField, Min(0)] private int maxOnGlobalMapEndurance;
        private BaseCharacter baseCharacter;

        #region baseCharacterImplemecation
        public Weapon ChosenWeapon => baseCharacter.ChosenWeapon;
        public string FirstName => baseCharacter.FirstName;
        public string SurName => baseCharacter.SurName;
        public Sprite Sprite => baseCharacter.Sprite;
        public CharacterSkills Skills => baseCharacter.Skills;
        public CharacterCharacteristics Characteristics => baseCharacter.Characteristics;
        public CharacterStatistics Statistics => baseCharacter.Statistics;

        public ChosenWeaponTypes ChosenWeaponType => baseCharacter.ChosenWeaponType;
        
        #endregion

        public ManBody ManBody => (ManBody) Body;

        public int MaxOnGlobalMapEndurance
        {
            get => ManBody.MaxOnGlobalMapEndurance;
            set => ManBody.MaxOnGlobalMapEndurance = value;
        }
        public int CurrentOnGlobalMapEndurance
        {
            get => ManBody.CurrentOnGlobalMapEndurance;
            set => ManBody.CurrentOnGlobalMapEndurance = value;
        }
        public event Action<MeleeWeapon> MeleeWeaponChanged;
        public event Action<Gun, GunType> GunsChanged;

        protected override void Awake()
        {
            base.Awake();
            baseCharacter = GetComponent<BaseCharacter>();
            MaxOnGlobalMapEndurance = maxOnGlobalMapEndurance;
            CurrentOnGlobalMapEndurance = maxOnGlobalMapEndurance;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            baseCharacter.ChosenWeaponChanged += BaseCharacterOnChosenWeaponChanged;
            TurnController.Instance.TurnEnded += OnTurnEnd;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            baseCharacter.ChosenWeaponChanged -= BaseCharacterOnChosenWeaponChanged;
            TurnController.Instance.TurnEnded -= OnTurnEnd;
        }
        
        private void BaseCharacterOnChosenWeaponChanged(Weapon oldWeapon, Weapon newWeapon)
        {
            ChosenWeaponChanged?.Invoke(oldWeapon, newWeapon);
        }



        public Gun PrimaryGun
        {
            get => baseCharacter.PrimaryGun;
            set
            {
                var oldGun = baseCharacter.PrimaryGun;
                baseCharacter.PrimaryGun = value;
                GunsChanged?.Invoke(oldGun, GunType.PrimaryGun);
            }
        }

        public Gun SecondaryGun
        {
            get => baseCharacter.SecondaryGun;
            set
            {
                var oldGun = baseCharacter.SecondaryGun;
                baseCharacter.SecondaryGun = value;
                GunsChanged?.Invoke(oldGun, GunType.SecondaryGun);
            }
        }

        public MeleeWeapon MeleeWeapon
        {
            get => baseCharacter.MeleeWeapon;
            set
            {
                baseCharacter.MeleeWeapon = value;
                MeleeWeaponChanged?.Invoke(baseCharacter.MeleeWeapon);
            }
        }

        public event Action<Weapon, Weapon> ChosenWeaponChanged;


        public void SetChosenWeapon(ChosenWeaponTypes type) => baseCharacter.SetChosenWeapon(type);

        public IEnumerable<Weapon> GetWeapons() => baseCharacter.GetWeapons();
        public void OnTurnEnd()
        {
            ManBody.OnTurnEnd();
            foreach (var bodyPart in ManBody.BodyParts)
            {
                bodyPart.Health.OnTurnEnd();
            }

            Statistics.daysAlive += 1;
            CurrentOnGlobalMapEndurance = MaxOnGlobalMapEndurance;
        }

        public void Eat(EatableFood food)
        {
            ManBody.Energy += food.Data.DeltaEnergy;
            ManBody.Water += food.Data.DeltaWater;
            ManBody.Hunger += food.Data.DeltaHunger;
            Destroy(food.gameObject);
        }

        public CharacterState CreateState()
        {
            return baseCharacter.CreateState();
        }

        public void Restore(CharacterState state)
        {
            baseCharacter.Restore(state);
        }
    }
}