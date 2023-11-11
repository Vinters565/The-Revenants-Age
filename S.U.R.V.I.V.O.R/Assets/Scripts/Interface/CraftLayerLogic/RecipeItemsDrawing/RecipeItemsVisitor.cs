using TheRevenantsAge;
using UnityEngine;

namespace Interface.CraftLayerLogic.RecipeItemsDrawing
{
    public class RecipeItemsVisitor : IRecipeDrawersVisitor<BaseCraftableItemDrawer>
    {
        private SingleCraftableItemDrawer SinglePrefab => Resources.Load<SingleCraftableItemDrawer>("Interface/Prefabs/Craft/Item");
        private ListCraftableItemsDrawer ListPrefab => Resources.Load<ListCraftableItemsDrawer>("Interface/Prefabs/Craft/ListItem");
        
        public BaseCraftableItemDrawer Visit(CraftRequiredList list)
        {
            var drawerObject = Object.Instantiate(ListPrefab.gameObject);
            var drawer =  drawerObject.GetComponent<ListCraftableItemsDrawer>();
            drawer.ReDrawItem(list);
            return drawer;
        }

        public BaseCraftableItemDrawer Visit(CraftRequiredItem item)
        {
            var drawerObject = Object.Instantiate(SinglePrefab.gameObject);
            var drawer =  drawerObject.GetComponent<SingleCraftableItemDrawer>();
            drawer.ReDrawItem(item);
            return drawer;
        }
    }
}