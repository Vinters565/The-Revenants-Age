using Inventory;
using UnityEngine;
using UnityEngine.UI;

public class LootAmountButtonLogic : MonoBehaviour
{
    private Button button;
    [SerializeField] private int LootAmount;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private InterfaceController interfaceController;
    [SerializeField] private GameObject LootAmountButtonsLayer;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        var chosenGroup = TheRevenantsAge.GlobalMapController.ChosenGroup;
        if (!chosenGroup.IsCanLoot) return;
        if (chosenGroup.Location.Data.CheckFight()) return;
        interfaceController.SetGroupLayerActive();
        foreach (var item in chosenGroup.Loot(LootAmount))
        {
            if (item == null) continue;
            inventoryController.SelectedInventoryGrid = LocationInventory.Instance.LocationInventoryGrid;
            inventoryController.AddItemToInventory(item);
        }

        chosenGroup.IsLooting = true;
    }
}