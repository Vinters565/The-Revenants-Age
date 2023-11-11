using System;
using System.Collections;
using System.Collections.Generic;
using TheRevenantsAge;

using UnityEngine;

public class ChosenCharacterCardLogic : MonoBehaviour
{
    [SerializeField] private FightCharacteristicsDrawer fightCharacteristicsDrawer;
    [SerializeField] private FightCharacterCard chosenCharacterCard;
    [SerializeField] private GameObject ifInactivePanel;
    
    private IFightCharacter currentCharacter;

    public IFightCharacter CurrentCharacter
    {
        get => currentCharacter;
        set
        {
            currentCharacter = value;
            ifInactivePanel.SetActive(value == null);
            if(value == null) return;
            fightCharacteristicsDrawer.CurrentCharacter = currentCharacter;
            chosenCharacterCard.CurrentCharacter = currentCharacter;
        }
    }
}
