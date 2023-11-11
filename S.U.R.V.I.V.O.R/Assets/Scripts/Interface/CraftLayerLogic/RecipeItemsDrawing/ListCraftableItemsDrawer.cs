using System;
using System.Collections.Generic;
using System.Linq;
using TheRevenantsAge;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.CraftLayerLogic
{
    public class ListCraftableItemsDrawer : BaseCraftableItemDrawer
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private LayoutGroup elementsLayoutGroup;
        [SerializeField] private RequiredItemDrawer requiredItemDrawerPrefab;
        [SerializeField] private Image imageOfCategory;
        [SerializeField] private TMP_Text itemAmount;

        private CraftRequiredList list;
        private bool isInited;
        
        private Dictionary<BaseItem, bool> havingItems;
        private Dictionary<BaseItem, RequiredItemDrawer> drawers;

        public void ReDrawItem(CraftRequiredList list)
        {
            this.list = list;
            imageOfCategory.sprite = list.Sprite;
            var size = list.Sprite.rect.size;
            imageOfCategory.gameObject.transform.localScale = size.x < size.y 
                ? new Vector3(size.x / size.y, 1, 1) 
                : new Vector3(1, size.y / size.x, 1);
            itemAmount.text = list.AmountOfItems.ToString();
        }
        
        public void OpenCloseListOfItems()
        {
            if(elementsLayoutGroup.IsActive())
            {
                CloseList();
                return;
            }
            OpenList();
        }

        private void ReDrawList()
        {
            CheckAllItems();
            foreach (var pair in drawers)
            {
                var drawer = pair.Value;
                var item = pair.Key;
                drawer.ReDraw(item, havingItems[item]);
            }
        }
        
        private void Init()
        {
            havingItems = new Dictionary<BaseItem, bool>();
            foreach (var listItem in list.Items)
            {
                havingItems.Add(listItem,false);
            }
            CheckAllItems();
            drawers = new();
            foreach (var item in list.Items)
            {
                var instance = Instantiate(requiredItemDrawerPrefab.gameObject, elementsLayoutGroup.transform);
                var requiredItemDrawer = instance.GetComponent<RequiredItemDrawer>();
                drawers.Add(item, requiredItemDrawer);
            }

            isInited = true;
        }

        private void CheckAllItems()
        {
            foreach (var listItem in list.Items)
            {
                havingItems[listItem] = false;
            }
            
            foreach (var item in GlobalMapController.ChosenGroup.GetAllGroupItemsByType<BaseItem>())
            {
                var equalItem = list.Items.FirstOrDefault(x => x.Equals(item));
                if (equalItem != null)
                {
                    havingItems[equalItem] = true;
                }
            }
        }
        
        private void OpenList()
        {
            if (!isInited)
            {
                Init();
            }
            ReDrawList();
            scrollRect.gameObject.SetActive(true);
        }

        private void CloseList()
        {
            scrollRect.gameObject.SetActive(false);
        }
    }
}