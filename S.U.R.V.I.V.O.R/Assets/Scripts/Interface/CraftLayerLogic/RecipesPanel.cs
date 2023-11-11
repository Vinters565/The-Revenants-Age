using System;
using System.Collections.Generic;
using System.Linq;
using Interface.CraftLayerLogic;
using Inventory;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.UI;

namespace TheRevenantsAge
{
    public class RecipesPanel : MonoBehaviour
    {

        [SerializeField] private RecipeManager recipeManager;
        [SerializeField] private RecipeDrawer choosenRecipe;
        [SerializeField] private GameObject RecipePrefab;
        [SerializeField] private GameObject RecipePanel;
        [SerializeField] private AviableToolsDrawer aviableToolsDrawer;
        
        public bool ShouldDrwOnlyAviableRecipes { get; set; }

        public Recipe CurrentRecipe { get; private set; }

        public void ReDrawRecipes(ICharacter character, IEnumerable<Recipe> RecipesToCheck = null)
        {
            var recipesToCheck = RecipesToCheck == null ? recipeManager.allRecipes.ToList() : RecipesToCheck.ToList();
            if (character == null || !gameObject.activeInHierarchy) return;
            var recipeToDelete = RecipePanel.GetComponentsInChildren<RecipeDrawer>();
            foreach (var recipe in recipeToDelete)
            {
                recipe.gameObject.SetActive(false);
            }

            var allowedTools = GlobalMapController.ChosenGroup.GetAllGroupItemsByType<Tool>().SelectMany(x => x.ToolTypeList).Distinct();
            
            aviableToolsDrawer.ReDrawAllTools(allowedTools.ToList());
            
            var allGroupItems = GlobalMapController.ChosenGroup.GetAllGroupItemsByType<BaseItem>();

            var allowedRecipes = new List<Recipe>();

            var recipes = new List<Recipe>();
            
            foreach (var recipe in recipesToCheck)
            {
                if (!(recipe.requiredCookingLevel <= character.Skills.Cooking.CurrentDishesLevel + 1) ||
                    !(recipe.requiredCraftLevel <= character.Skills.Crafting.CurrentCraftingLevel + 1) ||
                    !(recipe.requiredMedicineLevel <= character.Skills.Healing.CurrentHealingLevel + 1)) continue;
                
                
                if(recipe.requiredToolsToCraft.Count == 0 || recipe.requiredToolsToCraft.All(tool => allowedTools.Contains(tool)))
                {
                    if (recipe.requiredBaseItems.All(item =>
                        {
                            var amountOfAcceptedItems = allGroupItems.Where(item.IsItemAcceptedForRequire).ToArray().Length;
                            return amountOfAcceptedItems >= item.AmountOfItems;
                        }))
                    {
                        allowedRecipes.Add(recipe);
                        continue;
                    }
                }
                recipes.Add(recipe);
            } 
            
            DrawReadyRecipes(allowedRecipes, true);
            if(!ShouldDrwOnlyAviableRecipes)
                DrawReadyRecipes(recipes, false);
        }

        private void DrawReadyRecipes(IEnumerable<Recipe> recipes, bool canCraft)
        {
            foreach (var recipe in recipes)
            {
                var recipeDrawer = recipe.RecipeDrawer;
                if(recipeDrawer == null)
                {
                    recipeDrawer = Instantiate(RecipePrefab, RecipePanel.transform, true).GetComponent<RecipeDrawer>();
                    recipeDrawer.Init(recipe, canCraft, this);
                    recipe.RecipeDrawer = recipeDrawer;
                }
                else
                {
                    recipeDrawer.gameObject.SetActive(true);
                    recipeDrawer.transform.SetParent(RecipePanel.transform);
                    recipeDrawer.SetRecipeAviability(canCraft);
                }
            }
        }
        
        public void ChooseRecipe(Recipe recipe)
        {
            choosenRecipe.Init(recipe,false,this);
            CurrentRecipe = recipe;
        }
    }
}