using System;
using Interface.GroupLayerLogic;
using Interface.PlayerInfoLayer;
using TheRevenantsAge;
using UnityEngine;

[Obsolete]
public class PlayerCartLogic : MonoBehaviour
{
    [SerializeField] private ChangeCharacterOnButtonInfo nextCharacterButton;
    [SerializeField] private ChangeCharacterOnButtonInfo previousCharacterButton;
    [SerializeField] private PlayerLayerLogic playerLayerLogic;

    private IGlobalMapCharacter currentCharacter; 
    private IGlobalMapCharacter previousCharacter; 
    private IGlobalMapCharacter nextCharacter;

    public IGlobalMapCharacter CurrentCharacter
    {
        set
        {
            playerLayerLogic.CurrentCharacter = value;
            currentCharacter = value;
        }
    }

    public IGlobalMapCharacter PreviousCharacter
    {
        set
        {
            previousCharacterButton.CurrentCharacter = value;
            previousCharacter = value;
            previousCharacterButton.gameObject.SetActive(previousCharacter != currentCharacter);
        }
    }

    public IGlobalMapCharacter NextCharacter
    {
        set
        {
            nextCharacterButton.CurrentCharacter = value;
            nextCharacter = value;
            nextCharacterButton.gameObject.SetActive(previousCharacter != currentCharacter);
        }
    }

    public void ReDraw(PlayerCartReDrawInfo cartInfo)
    {
        CurrentCharacter = cartInfo.currentCharacter;
        PreviousCharacter = cartInfo.previousCharacter;
        NextCharacter = cartInfo.nextCharacter;
    }

    public void Init(PlayerCartReDrawInfo cart)
    {
        nextCharacterButton.Init();
        previousCharacterButton.Init();
        ReDraw(cart);
    }

    public void DoSomethingIfYouAreNotNeeded()
    {
        gameObject.SetActive(false);
    }
}
