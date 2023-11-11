using System;
using TheRevenantsAge;
using TMPro;
using UnityEngine;

public class FightCharacteristicsDrawer : MonoBehaviour
{
    private IFightCharacter currentCharacter;

    [SerializeField] private TMP_Text initiative;
    [SerializeField] private TMP_Text movingDistance;
    [SerializeField] private TMP_Text accuracy;
    [SerializeField] private TMP_Text shootSpeed;
    [SerializeField] private TMP_Text fireDistance;
    [SerializeField] private TMP_Text meleeDamage;


    public IFightCharacter CurrentCharacter
    {
        get => currentCharacter;
        set
        {
            UnsubscribeCharacterEvents();
            currentCharacter = value;
            SubscribeCharacterEvents();
        }
    }

    private void UnsubscribeCharacterEvents()
    {
        if (currentCharacter == null) return;
        currentCharacter.ChosenWeaponChanged -= OnChosenWeaponChanged;
    }

    private void SubscribeCharacterEvents()
    {
        if (currentCharacter == null) return;
        currentCharacter.ChosenWeaponChanged += OnChosenWeaponChanged;
    }

    private void OnChosenWeaponChanged(Weapon arg1, Weapon arg2)
    {
        if (arg2 == null)
            return; 
        
        var initiativeVar = Math.Round(currentCharacter.Initiative * ((currentCharacter.Skills.GetGunHandlingSkill(arg2.HandlingType).CurrentHandlingLevel.DeltaErgo + arg2.Ergonomics) / 100));
        var movingDistanceVar = currentCharacter.SpeedInFightScene;
        var accuracyVar = currentCharacter.Characteristics.Accuracy + currentCharacter.Skills
            .GetGunHandlingSkill(arg2.HandlingType).CurrentHandlingLevel.DeltaAccuracy;
        var shootSpeedVar = 1;
        var fireDistanceVar = 0f;
        if (arg2 is Gun gun)
        {
            fireDistanceVar = (gun.Data.OptimalFireDistanceBegin + gun.Data.OptimalFireDistanceEnd) / 2;
        }

        var meleeDamageVar = 0f;

        if (arg2 is MeleeWeapon meleeWeapon)
        {
            meleeDamageVar = meleeWeapon.Data.Damage;
        }

        initiative.text = initiativeVar.ToString();
        movingDistance.text = movingDistanceVar.ToString();
        accuracy.text = accuracyVar.ToString();
        shootSpeed.text = shootSpeedVar.ToString();
        fireDistance.text = fireDistanceVar.ToString();
        meleeDamage.text = meleeDamageVar.ToString();
    }
}