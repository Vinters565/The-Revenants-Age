using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.CraftLayerLogic
{
    public class CraftLayerLogic : MonoBehaviour
    {
        [SerializeField] private CrafterCardLogic crafterCardLogic;
        [SerializeField] private RecipeManager recipeManager;
        [SerializeField] private RecipesPanel recipesPanel;
        [SerializeField] private InventoryGrid craftedInventory;
        [SerializeField] private AviableToolsDrawer aviableToolsDrawer;
        
        [SerializeField] private Button allSortButton;
        [SerializeField] private Button craftSortButton;
        [SerializeField] private Button medSortButton;
        [SerializeField] private Button cookSortButton;
        [SerializeField] private Toggle shouldDrawOnlyAviableRecipesToggle;

        public void Init()
        {
            crafterCardLogic.Init();
            crafterCardLogic.CharactersList = TheRevenantsAge.GlobalMapController.ChosenGroup.CurrentGroupMembers.ToList();

            allSortButton.onClick.AddListener(() =>
            {
                recipesPanel.ReDrawRecipes(crafterCardLogic.CurrentCharacter);
            });
            
            craftSortButton.onClick.AddListener(() =>
            {
                recipesPanel.ReDrawRecipes(crafterCardLogic.CurrentCharacter, recipeManager.allRecipes
                    .Where(x => 
                        x.requiredCraftLevel >= x.requiredCookingLevel &&
                        x.requiredCraftLevel >= x.requiredMedicineLevel));
            });
            
            medSortButton.onClick.AddListener(() =>
            {
                recipesPanel.ReDrawRecipes(crafterCardLogic.CurrentCharacter, recipeManager.allRecipes
                    .Where(x => 
                        x.requiredMedicineLevel >= x.requiredCraftLevel &&
                        x.requiredMedicineLevel >= x.requiredCookingLevel));
            });
            
            cookSortButton.onClick.AddListener(() =>
            {
                recipesPanel.ReDrawRecipes(crafterCardLogic.CurrentCharacter, recipeManager.allRecipes
                    .Where(x => 
                        x.requiredCookingLevel >= x.requiredCraftLevel &&
                        x.requiredCookingLevel >= x.requiredMedicineLevel));
            });
            
            shouldDrawOnlyAviableRecipesToggle.onValueChanged.AddListener(x =>
            {
                recipesPanel.ShouldDrwOnlyAviableRecipes = x;
                recipesPanel.ReDrawRecipes(crafterCardLogic.CurrentCharacter);
            });
            
            aviableToolsDrawer.Init();
        }

        private void OnGroupChanged(Group oldGroup, Group newGroup)
        {
            crafterCardLogic.CharactersList = newGroup.CurrentGroupMembers.ToList();
        }

        private void OnCraftButtonClick()
        {
            var currentGroupItems =
                GlobalMapController.ChosenGroup.GetAllGroupItemsByType<BaseItem>().ToArray();
            var itemsToUseInCraft = new List<BaseItem>();
            var craftRequiredDict = new Dictionary<ICraftRequiredItem, int>();
            var canCraft = true;
            if(recipesPanel.CurrentRecipe == null) return;
            foreach (var craftRequiredItem in recipesPanel.CurrentRecipe.requiredBaseItems)
            {
                craftRequiredDict[craftRequiredItem] = craftRequiredItem.AmountOfItems;
            }

            var keys = new ICraftRequiredItem[craftRequiredDict.Keys.Count];
            craftRequiredDict.Keys.CopyTo(keys,0);
            foreach (var item in currentGroupItems)
            {
                foreach (var key in keys)
                {
                    if(craftRequiredDict[key] == 0)
                        continue;
                    if (!key.IsItemAcceptedForRequire(item)) continue;
                    craftRequiredDict[key] -= 1;
                    itemsToUseInCraft.Add(item);
                }
            }


            foreach (var val in craftRequiredDict.Values)
            {
                if (val != 0)
                {
                    canCraft = false;
                }
            }


            if (!canCraft)
            {
                recipesPanel.ReDrawRecipes(crafterCardLogic.CurrentCharacter);
                return;
            }
            
            foreach (var item in itemsToUseInCraft)
            {
                Destroy(item.gameObject);
            }

            foreach (var resultItem in recipesPanel.CurrentRecipe.resultItems)
            {
                for (int i = 0; i < resultItem.amountOfItems; i++)
                {
                    var item = Instantiate(resultItem.requiredItem);
                    var invItem = item.GetComponent<InventoryItem>();
                    craftedInventory.InsertItem(invItem);
                }
            }

            StartCoroutine(ReDrawRecipes());
        }

        private IEnumerator ReDrawRecipes()
        {
            yield return new WaitForSeconds(0.5f);
            recipesPanel.ReDrawRecipes(crafterCardLogic.CurrentCharacter);
        }
        
        private void OnEnable()
        {
            GlobalMapController.ChosenGroupChange += OnGroupChanged;
        }

        private void OnDisable()
        {
            GlobalMapController.ChosenGroupChange -= OnGroupChanged;
        }
    }
}