
using TheRevenantsAge;
using UnityEngine;

public class FightScenePlayerController : MonoBehaviour
{
    private FightSceneCharacter currentPlayerCharacter;
    private AttackType attackType = AttackType.DefaultAttack;

    private void Start()
    {
        FightSceneController.Instance.chosenPlayerCharacterChanged.AddListener(OnPlayerCharacterChanged);
    }

    private void OnDisable()
    {
        FightSceneController.Instance.chosenPlayerCharacterChanged.RemoveListener(OnPlayerCharacterChanged);
    }

    private void OnPlayerCharacterChanged(IFightCharacter newCharacter)
    {
        currentPlayerCharacter = (newCharacter as BaseFightCharacter).GetComponent<FightSceneCharacter>();
    }
    
    public void ChangeToPrimaryGun()
    {
        if (StateController.Instance.CanChangePhase())
        {
            var successfulChanged = currentPlayerCharacter.SetChosenWeapon(ChosenWeaponTypes.Primary);
            UIController.Instance.SetEnabledShootButton(successfulChanged); 
            UIController.Instance.SetEnabledHitButton(!successfulChanged);
            Debug.Log("ChangeGun to Primary");
            
        }
    }

    public void ChangeToSecondaryGun()
    {
        if (StateController.Instance.CanChangePhase())
        {
            var successfulChanged = currentPlayerCharacter.SetChosenWeapon(ChosenWeaponTypes.Secondary);
            Debug.Log("ChangeGun to Secondary");
            UIController.Instance.SetEnabledShootButton(successfulChanged); 
            UIController.Instance.SetEnabledHitButton(!successfulChanged);
        }
    }

    public void ChangeToMeleeWeapon()
    {
        if (StateController.Instance.CanChangePhase())
        {
            var successfulChanged = currentPlayerCharacter.SetChosenWeapon(ChosenWeaponTypes.Melee);
            if (successfulChanged)
            {
                UIController.Instance.SetEnabledShootButton(false);
                UIController.Instance.SetEnabledHitButton(true);
                Debug.Log("ChangeGun to Melee");
            }
        }
        // if (CurrentFightCharacter.Type == CharacterType.Ally)
        // {
        //     Debug.Log("Change to melee weapon");
        //     var character = CurrentFightCharacter.Entity as IFightCharacter;
        //     if (character.MeleeWeapon != null && character.ChosenWeaponType != ChosenWeaponTypes.Melee)
        //         Debug.Log("Change to melee weapon OK"); //TODO проверить правильность
        //     UIController.Instance.SetEnabledShootButton(false);
        //     UIController.Instance.SetEnabledHitButton(true);
        //     character.SetChosenWeapon(ChosenWeaponTypes.Melee);
        // }
    }

    public void SetHighMeleeAttack()
    {
        if (StateController.Instance.CanChangePhase())
        {
            currentPlayerCharacter.SetAttackType(AttackType.MeleeHighAttack);
        }
    }
    
    public void SetLowMeleeAttack()
    {
        if (StateController.Instance.CanChangePhase())
        {
            currentPlayerCharacter.SetAttackType(AttackType.MeleeLowAttack);
        }
    }
}
