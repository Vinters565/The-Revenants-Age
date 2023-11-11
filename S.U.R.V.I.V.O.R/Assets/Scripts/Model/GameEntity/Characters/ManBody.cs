using System;
using Extension;
using UnityEngine;

namespace TheRevenantsAge
{
    public sealed class ManBody : BodyWearableClothes
    {
        [SerializeField] [Min(0)] private int maxEnergy = 10;
        [SerializeField] [Min(0)] private int maxHunger = 10;
        [SerializeField] [Min(0)] private int maxWater = 10;

        //private int defaultColdResistance = 0;

        private int energy;
        private int hunger;
        private int water;
        
        private int minimalWaterRecoveryBorder = 8;
        private int minimalHungerRecoveryBorder = 8;
        private int minimalEnergyRecoveryBorder = 8;
        
        private int minimalWaterDebuffBorder = 2;
        private int minimalHungerDebuffBorder = 2;
        private int minimalEnergyDebuffBorder = 2;
        
        [SerializeField, ReadOnlyInspector] private int maxOnGlobalMapEndurance;
        [SerializeField, ReadOnlyInspector] private int currentOnGlobalMapEndurance;

        public event Action<PropertyManager> PlayerHungry;
        public event Action<PropertyManager> PlayerThirsty;
        public event Action<PropertyManager> PlayerTired;

        public event Action<int, int> EnergyChanged;
        public event Action<int, int> HungerChanged;
        public event Action<int, int> WaterChanged;
        

        protected override void Awake()
        {
            base.Awake();
            Energy = MaxEnergy;
            Hunger = MaxHunger;
            Water = MaxWater;
            CurrentOnGlobalMapEndurance = MaxOnGlobalMapEndurance;
        }
        
        public int Energy
        {
            get => energy;
            set
            {
                var buffer = energy;
                if (value <= 0)
                {
                    energy = 0;
                    PlayerTired?.Invoke(Health);
                }
                else if (value > maxEnergy)
                    energy = maxEnergy;
                else
                    energy = value;

                EnergyChanged?.Invoke(buffer,energy);
            }
        }

        public int Hunger
        {
            get => hunger;
            set
            {
                var buffer = hunger;
                if (value <= 0)
                {
                    hunger = 0;
                    PlayerHungry?.Invoke(Health);
                }
                else if (value > maxHunger)
                    hunger = maxHunger;
                else
                    hunger = value;

                HungerChanged?.Invoke(buffer,hunger);
            }
        }

        public int Water
        {
            get => water;
            set
            {
                var buffer = water;
                if (value <= 0)
                {
                    water = 0;
                    PlayerThirsty?.Invoke(Health);
                }
                else if (value > maxWater)
                    water = maxWater;
                else
                    water = value;

                WaterChanged?.Invoke(buffer,water);
            }
        }

        public int MaxEnergy
        {
            get => maxEnergy;
            set => maxEnergy = Math.Max(1, value);
        }

        public int MaxHunger
        {
            get => maxHunger;
            set => maxHunger = Math.Max(1, value);
        }

        public int MaxWater
        {
            get => maxWater;
            set => maxWater = Math.Max(1, value);
        }
        
        public int MinimalWaterRecoveryBorder
        {
            get => minimalWaterRecoveryBorder;
            set => minimalWaterRecoveryBorder = value;
        }

        public int MinimalHungerRecoveryBorder
        {
            get => minimalHungerRecoveryBorder;
            set => minimalHungerRecoveryBorder = value;
        }

        public int MinimalEnergyRecoveryBorder
        {
            get => minimalEnergyRecoveryBorder;
            set => minimalEnergyRecoveryBorder = value;
        }

        public int MinimalWaterDebuffBorder
        {
            get => minimalWaterDebuffBorder;
            set => minimalWaterDebuffBorder = value;
        }

        public int MinimalEnergyDebuffBorder
        {
            get => minimalEnergyDebuffBorder;
            set => minimalEnergyDebuffBorder = value;
        }

        public int MinimalHungerDebuffBorder
        {
            get => minimalHungerDebuffBorder;
            set => minimalHungerDebuffBorder = value;
        }

        public int MaxOnGlobalMapEndurance
        {
            get => maxOnGlobalMapEndurance;
            set
            {
                maxOnGlobalMapEndurance = Mathf.Max(0,value);
                currentOnGlobalMapEndurance = Math.Min(currentOnGlobalMapEndurance, value);
            }
        }

        public int CurrentOnGlobalMapEndurance
        {
            get => currentOnGlobalMapEndurance;
            set => currentOnGlobalMapEndurance = Mathf.Clamp(value, 0, maxOnGlobalMapEndurance);
        }

        public ManHead Head => (ManHead) BodyParts[0];
        public ManChest Chest => (ManChest) BodyParts[1];
        public ManStomach Stomach => (ManStomach) BodyParts[2];
        public ManArm LeftArm => (ManArm) BodyParts[3];
        public ManArm RightArm => (ManArm) BodyParts[4];
        public ManLeg LeftLeg => (ManLeg) BodyParts[5];
        public ManLeg RightLeg => (ManLeg) BodyParts[6];
        
        public Clothes GetClothByType(ClothType type)
        {
            switch (type)
            {
                case ClothType.Backpack:
                    return Chest.Backpack;
                case ClothType.Boots:
                    return RightLeg.Boots;
                case ClothType.Pants:
                    return RightLeg.Pants;
                case ClothType.Hat:
                    return Head.Hat;
                case ClothType.Jacket:
                    return Chest.Jacket;
                case ClothType.Underwear:
                    return Chest.Underwear;
                case ClothType.Vest:
                    return Chest.Vest;
            }

            return default;
        }
        
        public override BodyState CreateState()
        {
            var baseSave = base.CreateState(); 
            return new ManBodyState()
            {
                healthProperties = baseSave.healthProperties,
                bodyPartSaves = baseSave.bodyPartSaves,
                energy = Energy,
                hunger = hunger,
                water = water,
                onGlobalMapEndurance = CurrentOnGlobalMapEndurance,
                
                maxEnergy = maxEnergy,
                maxHunger = maxHunger,
                maxWater = maxWater,
                maxOnGlobalMapEndurance = MaxOnGlobalMapEndurance
            };
        }


        public override void Restore(BodyState state, bool isLoadTo3D = false)
        {
            base.Restore(state, isLoadTo3D);
            if (state is ManBodyState manBodySave)
            {
                MaxEnergy = manBodySave.maxEnergy;
                MaxHunger = manBodySave.maxHunger;
                MaxWater = manBodySave.maxWater;

                Energy = manBodySave.energy;
                Hunger = manBodySave.hunger;
                Water = manBodySave.water;
            }
        }

        public void OnTurnEnd()
        {
            Water--;
            Hunger--;
            Health.OnTurnEnd();
            CheckOnTurnEnd();
        }

        private void CheckOnTurnEnd()
        {
            //Если нужно вынести это в метод, то нужно позаботиться, чтобы обьекты HealthProperty создавались только когда нужно,
            //а не передавались с каждым вызовом метода

            if (Hunger <= MinimalHungerDebuffBorder){
                Health.AddProperty(new HungerDeBuff());
            }
            else if(Hunger >= minimalHungerRecoveryBorder)
            {
                Health.AddProperty(new HungerBuff());
            }
            if (Water <= MinimalWaterDebuffBorder)
            {
                Health.AddProperty(new WaterDeBuff());
            }
            else if(Water >= minimalWaterRecoveryBorder)
            {
                Health.AddProperty(new WaterBuff());
            }
            if (Energy <= MinimalEnergyDebuffBorder)
            {
                Health.AddProperty(new EnergyDeBuff());
            }
            else if(Energy >= minimalEnergyRecoveryBorder)
            {
                Health.AddProperty(new EnergyBuff());
            }
        }
    }
}