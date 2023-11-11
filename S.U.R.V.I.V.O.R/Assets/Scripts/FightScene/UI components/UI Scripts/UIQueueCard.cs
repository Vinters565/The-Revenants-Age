using System;
using System.Collections;
using System.Collections.Generic;
using TheRevenantsAge;
using TMPro;
using UnityEngine;

public class UIQueueCard : MonoBehaviour
{
    public FightSceneCharacter CurrentFightSceneCharacter {get; set;}
    
    public void SetFightSceneCharacter(FightSceneCharacter fightSceneCharacter)
    {
        CurrentFightSceneCharacter = fightSceneCharacter;
        if (fightSceneCharacter.Type == CharacterType.Ally)
        {
            var character = fightSceneCharacter.Entity as IFightCharacter;
            transform.Find("FightCharacterName").GetComponent<TMP_Text>().text =
                $"{character.FirstName} {character.SurName}";
        }
        else
            transform.Find("FightCharacterName").GetComponent<TMP_Text>().text = "Противник";
    }
}
