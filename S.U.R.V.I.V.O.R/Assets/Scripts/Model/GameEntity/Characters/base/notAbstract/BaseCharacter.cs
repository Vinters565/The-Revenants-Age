// ReSharper disable Unity.NoNullPropagation
using System;
using System.Collections.Generic;
using DataBase;
using Inventory.SpecialCells;
using UnityEngine;


namespace TheRevenantsAge
{
    public class BaseCharacter: BaseEntityWithWeapon, ICharacter
    {
        public static GameObject Default2DPrefab;
        public static GameObject Default3DPrefab;
                    
        private string address2D;
        private string address3D;
        
        [SerializeField] private string firstName;
        [SerializeField] private string surname;
        [SerializeField] private Sprite sprite;
        
        private ChosenWeaponTypes chosenWeaponType;
        
        private CharacterSkills skills;
        private CharacterStatistics characterStatistics;
        private CharacterCharacteristics characteristics;
        public Gun PrimaryGun { get; set; }
        public Gun SecondaryGun { get; set; }
        public MeleeWeapon MeleeWeapon { get; set; }

        public ChosenWeaponTypes ChosenWeaponType => chosenWeaponType;
        public event Action<Weapon, Weapon> ChosenWeaponChanged;

        public string FirstName => firstName;
        public string SurName => surname;
        public Sprite Sprite => sprite;
        
        public CharacterSkills Skills => skills;
        public CharacterCharacteristics Characteristics => characteristics;
        public CharacterStatistics Statistics => characterStatistics;
        public ManBody ManBody => (ManBody) Body;

        protected override void Awake()
        {
            Default2DPrefab = Resources.Load<GameObject>($"Characters/NEW/DefaultCharacter{Game.PREFAB_2D_POSTFIX}");
            Default3DPrefab = Resources.Load<GameObject>($"Characters/NEW/DefaultCharacter{Game.PREFAB_3D_POSTFIX}");
            base.Awake();
            skills = new CharacterSkills(this);
            characterStatistics = new CharacterStatistics();
            characteristics = new CharacterCharacteristics();
            InitializeResourceReferences();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            ManBody.HpChanged += OnHpChanged;
            ManBody.HungerChanged += OnFoodChanged;
            ManBody.WaterChanged += OnWaterChanged;
            ManBody.EnergyChanged += OnEnergyChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ManBody.HpChanged -= OnHpChanged;
            ManBody.HungerChanged -= OnFoodChanged;
            ManBody.WaterChanged -= OnWaterChanged;
            ManBody.EnergyChanged -= OnEnergyChanged;
        }

        public override IEnumerable<Weapon> GetWeapons()
        {
            if (PrimaryGun != null)
                yield return PrimaryGun;
            if (SecondaryGun != null)
                yield return SecondaryGun;
            if (MeleeWeapon != null)
                yield return MeleeWeapon;
        }

        public void SetChosenWeapon(ChosenWeaponTypes type)
        {
            Weapon oldWeapon = ChosenWeapon;
            
            if (oldWeapon != null)
            {
                
            }

            chosenWeaponType = type;

            switch (chosenWeaponType)
            {
                case ChosenWeaponTypes.Primary:
                    ChosenWeapon = PrimaryGun;
                    break;
                case ChosenWeaponTypes.Secondary:
                    ChosenWeapon = SecondaryGun;
                    break;
                case ChosenWeaponTypes.Melee:
                    ChosenWeapon = MeleeWeapon;
                    break;
                case ChosenWeaponTypes.None:
                    ChosenWeapon = null;
                    break;
            }


            Weapon newWeapon = ChosenWeapon;

            if (newWeapon != null)
            {
                
            }

            ChosenWeaponChanged?.Invoke(oldWeapon, newWeapon);
        }

        private void InitializeResourceReferences()
        {
            var address = GetComponent<Pointer>().Address;
            var dualGenerator = new DualAddressGenerator();
            if (string.IsNullOrEmpty(address))
                throw new Exception($"{gameObject.name} не адрессован!");
            var result = dualGenerator.GenerateDualAddress(address);
            address2D = result.Item1;
            address3D = result.Item2;
        }
        
        public CharacterState CreateState()
        {
            return new CharacterState()
            {
                characterStatistics = characterStatistics,
                address2D = address2D,
                address3D = address3D,
                firstName = FirstName,
                surName = SurName,
                manBody = (ManBodyState) ManBody.CreateState(),
                skills = skills.CreateState(),
                hat = ManBody.Head.Hat?.GetComponent<BaseItem>().CreateState(),
                underwear = ManBody.Chest.Underwear?.GetComponent<BaseItem>().CreateState(),
                jacket = ManBody.Chest.Jacket?.GetComponent<BaseItem>().CreateState(),
                backpack = ManBody.Chest.Backpack?.GetComponent<BaseItem>().CreateState(),
                vest = ManBody.Chest.Vest?.GetComponent<BaseItem>().CreateState(),
                boots = ManBody.LeftLeg.Boots?.GetComponent<BaseItem>().CreateState(),
                pants = ManBody.LeftLeg.Pants?.GetComponent<BaseItem>().CreateState(),
                primaryGun = PrimaryGun?.GetComponent<BaseItem>().CreateState(),
                secondaryGun = SecondaryGun?.GetComponent<BaseItem>().CreateState(),
                meleeWeapon = MeleeWeapon?.GetComponent<BaseItem>().CreateState(),
                characteristics = characteristics
            };
        }

        public void Restore(CharacterState state)
        {
            if (state == null) return;

            firstName = state.firstName;
            surname = state.surName;
            characterStatistics = state.characterStatistics;
            characteristics = state.characteristics;
            
            ClearOld();

            if (state.manBody != null)
                ManBody.Restore(state.manBody);
            if (state.skills != null)
                skills.Restore(state.skills);

            WearClothes(state.hat);
            WearClothes(state.underwear);
            WearClothes(state.jacket);
            WearClothes(state.backpack);
            WearClothes(state.vest);
            WearClothes(state.boots);
            WearClothes(state.pants);

            EquipWeapon(state.primaryGun);
            EquipWeapon(state.secondaryGun);
            EquipWeapon(state.meleeWeapon);

            void EquipWeapon(ItemState weaponState)
            {
                if (weaponState is null) return;

                var weaponObj = Game.Is3D
                    ? weaponState.InstantiateGameObj3D(SetThisItemOwner)
                    : weaponState.InstantiateGameObj2D(SetThisItemOwner);

                if (weaponObj is null) return;
                var gun = weaponObj.GetComponent<Gun>();
                if (gun != null)
                {
                    switch (gun.Data.GunType)
                    {
                        case GunType.PrimaryGun:
                            PrimaryGun = gun;
                            return;
                        case GunType.SecondaryGun:
                            SecondaryGun = gun;
                            return;
                    }
                }

                var meleeWeapon = weaponObj.GetComponent<MeleeWeapon>();
                if (meleeWeapon != null)
                    MeleeWeapon = meleeWeapon;
            }

            void WearClothes(ItemState clothesState)
            {
                if (clothesState is null) return;

                var clothesObj = Game.Is3D
                    ? clothesState.InstantiateGameObj3D(SetThisItemOwner)
                    : clothesState.InstantiateGameObj2D(SetThisItemOwner);

                if (clothesObj is null) return;
                var cloth = clothesObj.GetComponent<Clothes>();
                ManBody.Wear(cloth);
            }
        }

        private void SetThisItemOwner(BaseItem item)
        {
            item.ItemOwner = this;
        }

        private void ClearOld()
        {
            if (PrimaryGun != null)
                Destroy(PrimaryGun.gameObject);
            if (SecondaryGun != null)
                Destroy(SecondaryGun.gameObject);
            if (MeleeWeapon != null)
                Destroy(MeleeWeapon.gameObject);
            foreach (var clothe in ManBody.GetClothes())
                Destroy(clothe.gameObject);
        }

        #region Statistics
        private void OnHpChanged(float oldValue, float newValue)
        {
            var difference = newValue - oldValue;
            if(difference < 0)
                characterStatistics.lostHp -= difference;
        }
        private void OnFoodChanged(int oldValue, int newValue)
        {
            var difference = newValue - oldValue;
            if(difference < 0)
                characterStatistics.foodLost -= difference;
        }
        private void OnEnergyChanged(int oldValue, int newValue)
        {
            var difference = newValue - oldValue;
            if(difference < 0)
                characterStatistics.energyLost -= difference;
        }
        private void OnWaterChanged(int oldValue, int newValue)
        {
            var difference = newValue - oldValue;
            if(difference < 0)
                characterStatistics.waterLost -= difference;
        }
        

        #endregion
    }
}