using System;
using System.Collections.Generic;
using Interface.PlayerInfoLayer;
using TheRevenantsAge;
using UnityEngine;

public class CharacterChoseButtonsPanel : MonoBehaviour
{
    [SerializeField] private CharacterChoseButtonsPanel other;
    [SerializeField] private PlayerLayerLogic playerLayerLogic;
    [SerializeField] private CharactersSwitcherButtonDrawer firstCharacterButton;
    [SerializeField] private CharactersSwitcherButtonDrawer secondCharacterButton;
    [SerializeField] private CharactersSwitcherButtonDrawer thirdCharacterButton;
    [SerializeField] private CharactersSwitcherButtonDrawer fourthCharacterButton;

    private List<CharactersSwitcherButtonDrawer> buttonsIndexes;
    private IGlobalMapCharacter chosenCharacter;
    
    private event Action characterChosen;

    private IGlobalMapCharacter ChosenCharacter
    {
        get => chosenCharacter;
        set
        {
            chosenCharacter = value;
            ReDrawButtons();
            ReDrawPanel(value);
            characterChosen?.Invoke();
        }
    }

    private void Awake()
    {
        buttonsIndexes = new List<CharactersSwitcherButtonDrawer>
        {
            firstCharacterButton,
            secondCharacterButton,
            thirdCharacterButton,
            fourthCharacterButton
        };

        for (int i = 0; i < buttonsIndexes.Count; i++)
        {
            if(i < GlobalMapController.ChosenGroup.CurrentGroupMembers.Count)
                InitializeButton(buttonsIndexes[i], GlobalMapController.ChosenGroup.CurrentGroupMembers[i], i);
            else
            {
                InitializeButton(buttonsIndexes[i], null, i);
            }
        }
        ChoseFreeCharacter();
    }

    private void ReDrawButtons()
    {
        for (int i = 0; i < GlobalMapController.ChosenGroup.CurrentGroupMembers.Count; i++)
        {
            var currentCharacter = GlobalMapController.ChosenGroup.CurrentGroupMembers[i];
            var button = buttonsIndexes[i];
            var isCharacterChosen = currentCharacter == ChosenCharacter;
            var isCharacterChosenInOther = other != null && currentCharacter == other.ChosenCharacter;
            button.interactable = !(isCharacterChosen || isCharacterChosenInOther);
            button.SetChosen(isCharacterChosen);
        }
    }

    private void ReDrawPanel(IGlobalMapCharacter character)
    {
        playerLayerLogic.CurrentCharacter = character;
    }
    
    private void ChoseCharacter(int characterIndex)
    {
        ChosenCharacter = GlobalMapController.ChosenGroup.CurrentGroupMembers[characterIndex];
    }

    private void ChoseFreeCharacter()
    {
        playerLayerLogic.gameObject.SetActive(true);
        foreach (var character in GlobalMapController.ChosenGroup.CurrentGroupMembers)
        {
            if(other != null && other.ChosenCharacter == character) continue;
            chosenCharacter = character;
            ReDrawButtons();
            ReDrawPanel(character);
            return;
        }
        playerLayerLogic.gameObject.SetActive(false);
    }

    private void InitializeButton(CharactersSwitcherButtonDrawer button, IGlobalMapCharacter character, int index)
    {
        if (character != null)
        {
            button.SetAvailability(true);
            button.onClick.AddListener(() => ChoseCharacter(index));
            button.Character = character;
        }
        else
        {
            button.SetAvailability(false);
        }
    }
    
    private void OnEnable()
    {
        ReDrawButtons();
        ReDrawPanel(chosenCharacter);
        if (other != null)
        {
            other.characterChosen += ReDrawButtons;
        }
    }
    
    private void OnDisable()
    {
        if (other != null)
        {
            other.characterChosen -= ReDrawButtons;
        }
    }
}
