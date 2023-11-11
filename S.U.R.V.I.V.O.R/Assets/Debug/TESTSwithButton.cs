using System;
using TheRevenantsAge;
using UnityEngine;


public class TESTSwithButton : MonoBehaviour
{
    [SerializeField] private BaseCharacter character;
    [SerializeField] private Gun primaryWeapon;
    [SerializeField] private Gun secondaryWeapon;
    [SerializeField] private MeleeWeapon meleeWeapon;

    private void Awake()
    {
        character.PrimaryGun = primaryWeapon;
        character.SecondaryGun = secondaryWeapon;
        character.MeleeWeapon = meleeWeapon;
        
        character.GetComponent<CharacterAnimationController>().CharacterOnRestoreEnd();
    }

    public void ToPrimary()
    {
        character.SetChosenWeapon(ChosenWeaponTypes.Primary);
    }
    
    public void ToSecondary()
    {
        character.SetChosenWeapon(ChosenWeaponTypes.Secondary);
    }
    
    public void ToMelee()
    {
        character.SetChosenWeapon(ChosenWeaponTypes.Melee);
    }
    
    public void Remove()
    {
        character.SetChosenWeapon(ChosenWeaponTypes.None);
    }
}