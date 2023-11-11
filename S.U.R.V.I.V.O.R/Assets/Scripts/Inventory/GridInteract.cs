using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory
{
    [RequireComponent(typeof(InventoryGrid))]
    public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private InventoryController inventoryController;
        private InventoryGrid inventoryGrid;

        private void Awake()
        {
            inventoryController = InventoryController.Instance;
            inventoryGrid = GetComponent<InventoryGrid>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            inventoryController.SelectedInventoryGrid = inventoryGrid;
            inventoryController.isPointerUnderInventory = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryController.SelectedInventoryGrid = null;
            inventoryController.isPointerUnderInventory = false;
        }

        private void OnDisable()
        {
            inventoryController.SelectedInventoryGrid = null;
            inventoryController.isPointerUnderInventory = false;
        }
    }
}