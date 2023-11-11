using TheRevenantsAge;
using UnityEngine;
using UnityEngine.UI;

public class LootButtonScript : MonoBehaviour
{
    private Sprite defaultSprite;
    private Button button;
    
    [SerializeField] private Image image;
    [SerializeField] private GameObject LootAmountButtonsLayer;
    
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick); 
        GlobalMapController.ChosenGroup.GroupMovementLogic.GroupNodeChangedToHard += OnGroupLocationChangedToHard;
        GlobalMapController.ChosenGroupChange += OnGroupChange;
        defaultSprite = image.sprite;
    }

    void OnClick()
    {
        LootAmountButtonsLayer.SetActive(!LootAmountButtonsLayer.activeInHierarchy);
    }
    
    private void OnGroupLocationChangedToHard(HardNode oldNode, HardNode newNode)
    {
        if (newNode == null)
        {
            button.onClick.AddListener(OnClick);
            image.sprite = defaultSprite;
            button.onClick.RemoveListener(oldNode.OnLootButtonClick);
        }
        else
        {
            button.onClick.RemoveListener(OnClick);
            image.sprite = newNode.LootButtonSprite;
            button.onClick.AddListener(newNode.OnLootButtonClick);
        }
    }
    
    private void OnGroupChange(Group oldGroup, Group newGroup)
    {
        oldGroup.GroupMovementLogic.GroupNodeChangedToHard -= OnGroupLocationChangedToHard;
        newGroup.GroupMovementLogic.GroupNodeChangedToHard += OnGroupLocationChangedToHard;
    }
}
