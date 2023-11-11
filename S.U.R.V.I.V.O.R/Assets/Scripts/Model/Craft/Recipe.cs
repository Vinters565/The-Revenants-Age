using System.Collections.Generic;
using Interface.CraftLayerLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheRevenantsAge
{
    [System.Serializable]
    public class Recipe //В создании рецепта нельзя использовать одинаковые предметы, ломается счетчик. Нельзя 2 вервеки + 3 веревки
    {
        [SerializeField] public string recipeName;
        [FormerlySerializedAs("requiredItems")] [SerializeField] public List<CraftRequiredItem> requiredBaseItems;
        [SerializeField] public List<CraftRequiredList> requiredItemsLists;
        [SerializeField] public List<CraftRequiredItem> resultItems;
        [SerializeField] public List<ToolType> requiredToolsToCraft;
        [SerializeField] public int requiredCraftLevel;
        [SerializeField] public int requiredMedicineLevel;
        [SerializeField] public int requiredCookingLevel;
        
        private RecipeDrawer recipeDrawer;

        public RecipeDrawer RecipeDrawer
        {
            get => recipeDrawer;
            set => recipeDrawer = value;
        }
    }
}