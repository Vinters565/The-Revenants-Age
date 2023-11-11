using System;
using System.Collections;
using System.Collections.Generic;
using Interface.BodyIndicatorFolder;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightCharacterCard : MonoBehaviour
{

    [SerializeField] private Image characterPhoto;
    
    [SerializeField] private TMP_Text characterName;

    [SerializeField] private BodyIndicator characterBodyIndicator;

    [SerializeField] private ProgressBarScript characterHpProgressBar;
    
    private IFightCharacter currentCharacter;

    public IFightCharacter CurrentCharacter
    {
        get => currentCharacter;
        set
        {
            currentCharacter = value;
            characterBodyIndicator.Character = value;
            characterHpProgressBar.Init(currentCharacter.ManBody.MaxHp);
            characterHpProgressBar.SetValue(currentCharacter.ManBody.Hp);
            characterName.text = $"{currentCharacter.FirstName} {currentCharacter.SurName}";
            characterPhoto.sprite = currentCharacter.Sprite;
            currentCharacter.ManBody.HpChanged += OnHpChanged;
        }
    }

    private void OnHpChanged(float arg1, float arg2)
    {
        characterHpProgressBar.SetValue(currentCharacter.ManBody.Hp);
    }

    private void OnDestroy()
    {
        currentCharacter.ManBody.HpChanged -= OnHpChanged;
    }
}
