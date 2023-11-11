using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheRevenantsAge
{
    public class BaseFightCharacter: BaseFightEntity, IFightCharacter
    {
        public static Dictionary<ICharacter, List<ItemState>> MagazinesStatesToCharacters = new ();
        private BaseCharacter baseCharacter;

        [SerializeField] private int actionPoints;
        [SerializeField] private int turnPoints;
        
        
        private CharacterAnimationController characterAnimationController;
        private CharacterClothesController clothesController;

        #region BaseCharacterImplemecation
        public Weapon ChosenWeapon => baseCharacter.ChosenWeapon;
        public string FirstName => baseCharacter.FirstName;
        public string SurName => baseCharacter.SurName;
        public Sprite Sprite => baseCharacter.Sprite;
        public CharacterSkills Skills => baseCharacter.Skills;
        public CharacterCharacteristics Characteristics => baseCharacter.Characteristics;
        public CharacterStatistics Statistics => baseCharacter.Statistics;
        public Gun PrimaryGun => baseCharacter.PrimaryGun;
        public Gun SecondaryGun => baseCharacter.SecondaryGun;
        public MeleeWeapon MeleeWeapon => baseCharacter.MeleeWeapon;
        public ChosenWeaponTypes ChosenWeaponType => baseCharacter.ChosenWeaponType;

        public override Aimer Aimer
        {
            get
            {
                if (baseCharacter.ChosenWeapon == null)
                    return null;
                return baseCharacter.ChosenWeapon.Aimer;
            }
        }

        #endregion
        
        public int ActionPoints => actionPoints;
        public int TurnPoints => turnPoints;
        public ManBody ManBody => (ManBody) Body;
        public event Action<Weapon, Weapon> ChosenWeaponChanged;
        
        public event Action RestoreEnd;

        public void ReloadChosenWeapon()
        {
            if (ChosenWeapon is Gun gun)
            {
                if (MagazinesStatesToCharacters.ContainsKey(baseCharacter))
                {
                    var magazinesStates = MagazinesStatesToCharacters[baseCharacter];
                    var magazineState = magazinesStates.First(m  => m != null);
                    var magazine = magazineState.InstantiateGameObj3D((item) => item.ItemOwner = baseCharacter);
                    if(magazine != null && magazine.GetComponent<Magazine>())
                        gun.Reload(magazine.GetComponent<Magazine>());
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            baseCharacter = GetComponent<BaseCharacter>();
            characterAnimationController = GetComponent<CharacterAnimationController>();
            clothesController = GetComponent<CharacterClothesController>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            baseCharacter.ChosenWeaponChanged += OnChosenWeaponChanged;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            baseCharacter.ChosenWeaponChanged -= OnChosenWeaponChanged;
        }
        
        private void OnChosenWeaponChanged(Weapon oldWeapon, Weapon newWeapon)
        {
            ChosenWeaponChanged?.Invoke(oldWeapon, newWeapon);
        }

        public IEnumerable<Weapon> GetWeapons() => baseCharacter.GetWeapons();

        public void SetChosenWeapon(ChosenWeaponTypes type)
        {
            if (!characterAnimationController.IsCanSetWeapon) return;
            baseCharacter.SetChosenWeapon(type);
        }

        public CharacterState CreateState() => baseCharacter.CreateState();

        public override void Attack(Vector3 targetPoint, AttackType type)
        {
            if (baseCharacter.ChosenWeaponType == ChosenWeaponTypes.Melee
                || baseCharacter.ChosenWeaponType == ChosenWeaponTypes.None)
            {
                MeleeAttack(type);
            }
            else
            {
                WeaponAttack(targetPoint);
            }

        }

        private void MeleeAttack(AttackType type)
        {
            if (type == AttackType.DefaultAttack)
                type = AttackType.MeleeLowAttack;
            
            if (type == AttackType.MeleeLowAttack)
                characterAnimationController.LowMeleeAttack();
            else if(type == AttackType.MeleeHighAttack)
                characterAnimationController.HighMeleeAttack();
            else
                Debug.LogError($"BaseFightCharacter | Attack | There is not melee attack type");
        }

        private void WeaponAttack(Vector3 targetPoint)
        {
            characterAnimationController.TriggerAttack();
            baseCharacter.ChosenWeapon.Attack(targetPoint, baseCharacter.Skills);
        }

        public void Restore(CharacterState state)
        {
            baseCharacter.Restore(state);
            foreach (var clothes in ManBody.GetClothes())
                clothesController.Wear3DClothes(clothes);
            
            characterAnimationController.EquipAllWeapons();
            
            RestoreEnd?.Invoke();
        }
    }
}