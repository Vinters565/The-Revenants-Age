using System;
using System.Collections.Generic;
using System.Linq;
using Interface.CraftLayerLogic.RecipeItemsDrawing;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Interface.CraftLayerLogic
{
    public class RecipeDrawer : MonoBehaviour
    {
        [SerializeField] private GameObject PlusPrefab;
        [SerializeField] private GameObject EuqalPrefab;
        [SerializeField] private Image RequireToolPrefab;
        [SerializeField] private Image itemsListImage;
        [SerializeField] private Image canCraftImage;
        [SerializeField] private Button chooseButton;
        [SerializeField] private LayoutGroup requiredToolsGroup;
        
        [SerializeField] private Sprite medicalRecipeSprite;
        [SerializeField] private Sprite craftRecipeSprite;
        [SerializeField] private Sprite cookRecipeSprite;
        
        [SerializeField] private Transform requiredItemsGroup;
        [SerializeField] private Transform resultItemsGroup;
        
        [SerializeField] private TextMeshProUGUI requiredCraftLevel;
        [SerializeField] private TextMeshProUGUI requiredMedicineLevel;
        [SerializeField] private TextMeshProUGUI requiredCookingLevel;
        [SerializeField] private TextMeshProUGUI recipeName;

        private RecipeItemsVisitor recipeItemsVisitor;
        
        public void Init(Recipe recipe, bool canCraftRecipe, RecipesPanel recipesPanel)
        {
            chooseButton.onClick.AddListener( () =>
            {
                recipesPanel.ChooseRecipe(recipe);
            });
            recipeItemsVisitor = new RecipeItemsVisitor();
            SetRecipeAviability(canCraftRecipe);
            
            itemsListImage.sprite = GetRecipeImageByType(recipe);
            
            requiredCookingLevel.text = recipe.requiredCookingLevel.ToString();
            requiredMedicineLevel.text = recipe.requiredMedicineLevel.ToString();
            requiredCraftLevel.text = recipe.requiredCraftLevel.ToString();
            recipeName.text = recipe.recipeName;
            var needs = recipe.requiredItemsLists.Concat(recipe.requiredBaseItems.Select(x => x as ICraftRequiredItem));
            DrawItemsToHorGroup(needs.ToList(),requiredItemsGroup.transform);
            Instantiate(EuqalPrefab, requiredItemsGroup.transform, false);
            DrawItemsToHorGroup(recipe.resultItems.Select(x => x as ICraftRequiredItem).ToList(),resultItemsGroup.transform);
            DrawRequiredTools(recipe);
        }

        public void SetRecipeAviability(bool canCraft)
        {
            if(canCraftImage != null)
                canCraftImage.color = canCraft ? Color.green : Color.red;
            chooseButton.gameObject.SetActive(canCraft);
        }


        private void DrawItemsToHorGroup(List<ICraftRequiredItem> requiredItems, Transform parentTransform)
        {
            foreach (var item in parentTransform.GetComponentsInChildren<BaseCraftableItemDrawer>())
            {
                Destroy(item.gameObject);
            }
            for (int i = 0; i < requiredItems.Count; i++)
            {
                if (i != 0)
                {
                    Instantiate(PlusPrefab, parentTransform, false);
                }
                var rcp = requiredItems[i];

                var drawer = rcp.Accept(recipeItemsVisitor);
                drawer.gameObject.transform.SetParent(parentTransform);
            }
        }

        private void DrawRequiredTools(Recipe recipe)
        {
            foreach (var toolType in recipe.requiredToolsToCraft)
            {
                var obj = Instantiate(RequireToolPrefab.gameObject, requiredToolsGroup.transform);
                var img = obj.GetComponent<Image>();
                img.sprite = Tool.GetSpriteByToolType(toolType).AviableSprite;
            }
        }

        private Sprite GetRecipeImageByType(Recipe recipe)
        {
            var maxValue = Math.Max(recipe.requiredCookingLevel, Math.Max(recipe.requiredCraftLevel, recipe.requiredMedicineLevel));
            
            if (maxValue == recipe.requiredCraftLevel)
            {
                return craftRecipeSprite;
            }
            
            if (maxValue == recipe.requiredCookingLevel)
            {
                return cookRecipeSprite;
            }

            if (maxValue == recipe.requiredMedicineLevel)
            {
                return medicalRecipeSprite;
            }

            return craftRecipeSprite;
        }
        
        private void OnDestroy()
        {
            chooseButton.onClick.RemoveAllListeners();
        }
    }
}