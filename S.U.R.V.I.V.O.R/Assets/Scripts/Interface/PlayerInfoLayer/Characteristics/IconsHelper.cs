using System;
using TheRevenantsAge;
using UnityEngine;

[System.Serializable]
public class IconsHelper : MonoBehaviour
{
    public static readonly Sprite Initiative =
        Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/initiative");

    public static readonly Sprite TravelRange = Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/far");
    
    public static readonly Sprite Accuracy = Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/accuracy");

    public static readonly Sprite ShootingRange =
        Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/fireDistance");

    public static readonly Sprite FireRate =
        Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/shoot-speed");

    public static readonly Sprite MeleeDamage = Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/melee");
    
    public static readonly Sprite Recoil = Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/pistol");

    public static readonly Sprite
        DishesLevel = Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/cooking");

    public static readonly Sprite
        CraftingLevel = Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/Craft");

    public static readonly Sprite ChanceToCutFish =
        Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/Fishing");

    public static readonly Sprite HealingLevel =
        Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/healing");

    public static readonly Sprite OptimalDistance =
        Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/fireDistance");

    public static readonly Sprite Ergo = Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/ergo");

    public static readonly Sprite ChanceToFindExtraItem =
        Resources.Load<Sprite>("Interface/Icons/Characteristics/Skills/chanceToFindExtraItem");

    public enum Characteristics
    {
        Initiative,
        TravelRange,
        Accuracy,
        ShootingRange,
        FireRate,
        MeleeDamage,
        Recoil,
        DishesLevel,
        CraftingLevel,
        ChanceToCutFish,
        HealingLevel,
        OptimalDistance,
        Ergo,
        ChanceToFindExtraItem,
        MaxPeopleInGroup,
        DeltaNoiseWhileLooting,
        DeltaNoiseWhileFighting,
        CanFreeLoot
    }

    public static Sprite GetCharacteristicIcon(Characteristics characteristic)
    {
        return characteristic switch
        {
            Characteristics.Initiative => Initiative,
            Characteristics.TravelRange => TravelRange,
            Characteristics.Accuracy => Accuracy,
            Characteristics.ShootingRange => ShootingRange,
            Characteristics.FireRate => FireRate,
            Characteristics.MeleeDamage => MeleeDamage,
            Characteristics.Recoil => Recoil,
            Characteristics.DishesLevel => DishesLevel,
            Characteristics.CraftingLevel => CraftingLevel,
            Characteristics.ChanceToCutFish => ChanceToCutFish,
            Characteristics.HealingLevel => HealingLevel,
            Characteristics.OptimalDistance => OptimalDistance,
            Characteristics.Ergo => Ergo,
            Characteristics.ChanceToFindExtraItem => ChanceToFindExtraItem,
            _ => Initiative
        };
    }

    public static Sprite GetHealthPropertyIcon(IHealthPropertyVisitor property)
    {
        if (property.GetType() == typeof(Bleeding)) //
        {
            return Resources.Load<Sprite>("Interface/Icons/Properties/Bleeding");
        }

        if (property.GetType() == typeof(Broking)) //
        {
            return Resources.Load<Sprite>("Interface/Icons/Properties/Broking");
        }

        if (property.GetType() == typeof(WaterBuff)) //
        {
            return Resources.Load<Sprite>("Interface/Icons/Properties/WaterBoost");
        }

        if (property.GetType() == typeof(HungerBuff)) //
        {
            return Resources.Load<Sprite>("Interface/Icons/Properties/FoodBoost");
        }

        if (property.GetType() == typeof(EnergyBuff)) //
        {
            return Resources.Load<Sprite>("Interface/Icons/Properties/energyBuff");
        }

        if (property.GetType() == typeof(WaterDeBuff)) //
        {
            return Resources.Load<Sprite>("Interface/Icons/Properties/Thirst");
        }

        if (property.GetType() == typeof(HungerDeBuff)) //
        {
            return Resources.Load<Sprite>("Interface/Icons/Properties/Hunger");
        }

        if (property.GetType() == typeof(EnergyDeBuff)) //
        {
            return Resources.Load<Sprite>("Interface/Icons/Properties/energyDebuff");
        }

        if (property.GetType() == typeof(Propitaling)) //
        {
            return Resources.Load<Sprite>("Interface/Icons/Properties/Propitaling");
        }

        return default;
    }
}