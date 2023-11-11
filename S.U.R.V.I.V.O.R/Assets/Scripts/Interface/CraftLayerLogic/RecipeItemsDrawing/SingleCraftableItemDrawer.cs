using Inventory;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.CraftLayerLogic
{
    public class SingleCraftableItemDrawer : BaseCraftableItemDrawer
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemAmount;

        public void ReDrawItem(CraftRequiredItem item)
        {
            itemImage.sprite = item.Sprite;
            var size = item.Sprite.rect.size;
            itemImage.gameObject.transform.localScale = size.x < size.y ? new Vector3((float)size.x / size.y, 1, 1) : new Vector3(1, (float)size.y / size.x, 1);
            itemAmount.text = item.AmountOfItems.ToString();
        }
    }
}