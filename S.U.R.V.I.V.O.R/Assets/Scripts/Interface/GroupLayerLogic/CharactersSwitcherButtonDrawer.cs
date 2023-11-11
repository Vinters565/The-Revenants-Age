using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharactersSwitcherButtonDrawer : MonoBehaviour
{
    private static Color32 selectedColor = new (157,245,124,255);
    private static Color32 unSelectedColor = new (255,255,255,255);
    
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text nameCharacterText;
    [SerializeField] private Image unAvailableImage;
        
    private IGlobalMapCharacter character;

    public IGlobalMapCharacter Character
    {
        get => character;
        set
        {
            character = value;
            Init();
        }
    }

    public bool interactable
    {
        get => button.interactable;
        set => button.interactable = value;
    }

    public Button.ButtonClickedEvent onClick => button.onClick;

    public void SetAvailability(bool isAvailable)
    {
        button.enabled = isAvailable;
        unAvailableImage.gameObject.SetActive(!isAvailable);
        nameCharacterText.gameObject.SetActive(isAvailable);
    }

    public void SetChosen(bool value)
    {
        button.image.color = value ? selectedColor : unSelectedColor;
    }
    
    private void Init()
    {
        nameCharacterText.text = $"{character.FirstName} {character.SurName}";
    }
}
