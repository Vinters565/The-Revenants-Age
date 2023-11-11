using System;
using System.Collections.Generic;
using Extension;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Inventory
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class InventoryGridBackground : MonoBehaviour
    {
        [SerializeField] private Transform enableSlotPrefab;
        [SerializeField] private Transform disableSlotPrefab;
        [SerializeField] private GameObject IfInactivePanel;
        [SerializeField] private InventoryGrid inventoryGrid;

        private List<Transform> listSlots = new ();
        private GridLayoutGroup grid;
        private RectTransform RectTransform => (RectTransform)transform;

        private void Awake()
        {
            grid = GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(InventoryGrid.REAL_TILE_SIZE, InventoryGrid.REAL_TILE_SIZE);
            grid.spacing = new Vector2(InventoryGrid.TILE_SPACING, InventoryGrid.TILE_SPACING);
            
        }

        public void DebugDrawGreenSquares()
        {
            var index = -1;
            for (var y = 0; y < inventoryGrid.MaxSize.y; y++)
            {
                if (y >= inventoryGrid.CurrentSize.y) break;
                for (var x = 0; x < inventoryGrid.MaxSize.x; x++)
                {
                    index++;
                    if (x >= inventoryGrid.CurrentSize.x) continue;
                    if(inventoryGrid.IsSlotFree(new Vector2Int(x,y)))
                        listSlots[index].GetComponent<Image>().color = Color.green;
                    else
                        listSlots[index].GetComponent<Image>().color = Color.red;
                }
            }
        }
        
        public void ReDrawBackground()
        {
            if (listSlots.Count > 0)
                DestroyAllSlots();
            bool shouldTurnOnIfInactivePanel = true;
            for (var y = 0; y < inventoryGrid.MaxSize.y; y++)
            {
                for (var x = 0; x < inventoryGrid.MaxSize.x; x++)
                {
                    Transform singleSlot;
                    if (x < inventoryGrid.CurrentSize.x && y < inventoryGrid.CurrentSize.y)
                    {
                        singleSlot = Instantiate(enableSlotPrefab, transform);
                        shouldTurnOnIfInactivePanel = false;
                    }
                    else
                    {
                        singleSlot = Instantiate(disableSlotPrefab, transform);
                    }
                    singleSlot.gameObject.SetActive(true);
                    listSlots.Add(singleSlot);
                }
            }
            
            RectTransform.sizeDelta = inventoryGrid.MaxSize * InventoryGrid.TileSize 
                                      + (Vector2Int.one * Math.Abs(InventoryGrid.TILE_SPACING));
            RectTransform.anchoredPosition = inventoryGrid.GetComponent<RectTransform>().anchoredPosition;
            if(IfInactivePanel is not null)
                IfInactivePanel.SetActive(shouldTurnOnIfInactivePanel);
        }

        private void DestroyAllSlots()
        {
            foreach (var slot in listSlots)
            {
                Destroy(slot.gameObject);
            }

            listSlots.Clear();
        }
    }
}