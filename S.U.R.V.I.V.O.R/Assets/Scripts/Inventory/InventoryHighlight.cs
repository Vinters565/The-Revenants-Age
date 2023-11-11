using UnityEngine;

namespace Inventory
{
    public class InventoryHighlight : MonoBehaviour
    {
        [SerializeField] private RectTransform highlighter;

        public void SetSize(InventoryItem targetItem)
        {
            var size = new Vector2(targetItem.Width * InventoryGrid.TileSize,
                targetItem.Height * InventoryGrid.TileSize);
            highlighter.sizeDelta = size;
        }

        public void SetPosition(InventoryGrid targetGrid, InventoryItem targetItem)
        {
            SetPosition(targetGrid, targetItem, targetItem.OnGridPositionX, targetItem.OnGridPositionY);
        }

        public void SetPosition(InventoryGrid targetGrid, InventoryItem targetItem, int posX, int posY)
        {
            var position = targetGrid.GetPositionOnGrid(targetItem, posX, posY);
            highlighter.localPosition = position;
        }

        public void Show(bool state)
        {
            highlighter.gameObject.SetActive(state);
        }

        public void SetParent(InventoryGrid targetGrid)
        {
            if(targetGrid == null) return;
            highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
            highlighter.SetAsLastSibling();
        }
    }
}