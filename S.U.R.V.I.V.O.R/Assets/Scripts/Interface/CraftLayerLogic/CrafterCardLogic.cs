using System.Collections.Generic;
using Interface.GroupLayerLogic;
using Interface.PlayerInfoLayer;
using TheRevenantsAge;
using UnityEngine;


namespace Interface.CraftLayerLogic
{
    public class CrafterCardLogic : MonoBehaviour
    {
        [SerializeField] private RecipesPanel recipesPanel;
        [SerializeField] private ChangeCrafterButtonLogic nextCharacterButton;
        [SerializeField] private ChangeCrafterButtonLogic previousCharacterButton;
        [SerializeField] private PlayerLayerLogic playerLayerLogic;
        [SerializeField] private CraftingSkillsInfo playerCraftingSkillsInfo;
        
        private IGlobalMapCharacter currentCharacter; 
        private IGlobalMapCharacter previousCharacter; 
        private IGlobalMapCharacter nextCharacter;

        public IGlobalMapCharacter CurrentCharacter
        {
            get => currentCharacter;
            private set
            {
                currentCharacter = value;
                playerCraftingSkillsInfo.CurrentCharacter = value;
                recipesPanel.ReDrawRecipes(value);
            }
        }

        private List<IGlobalMapCharacter> charactersList;
        public List<IGlobalMapCharacter> CharactersList
        {
            get => charactersList;
            set 
            {
                charactersList = value;
                OnListChanged();
            }
        }

        public void Init()
        {
            nextCharacterButton.Init();
            previousCharacterButton.Init();
            playerCraftingSkillsInfo.Init(null);
        }
        public void ChangeCharactersOnGroupLayer(bool isUp)
        {
            ChangeCardCharacter(isUp ? nextCharacter : previousCharacter);
        }

        private void ReDraw(PlayerCartReDrawInfo redrawable)
        {
            nextCharacterButton.CurrentCharacter = redrawable.nextCharacter;
            playerLayerLogic.CurrentCharacter = redrawable.currentCharacter;
            previousCharacterButton.CurrentCharacter = redrawable.previousCharacter;
        }
        
        private void ChangeCardCharacter(IGlobalMapCharacter currChar)
        {
            var currentCharacterIndex = 0;
            for (int i = 0; i < CharactersList.Count; i++)
            {
                var character = CharactersList[i];
                if (character == currChar)
                {
                    currentCharacterIndex = i;
                }
            }

            var prevChar = currentCharacterIndex == 0 ? CharactersList[^1] : CharactersList[currentCharacterIndex - 1];
            var nextChar = currentCharacterIndex != CharactersList.Count - 1 ? CharactersList[currentCharacterIndex + 1] : CharactersList[0];

            var reDrawInfo = new PlayerCartReDrawInfo(prevChar, currChar, nextChar);
            CurrentCharacter = currChar;
            previousCharacter = prevChar;
            nextCharacter = nextChar;
            ReDraw(reDrawInfo);
        }

        private void OnListChanged()
        {
            if (CharactersList.Count==0) return;
            ChangeCardCharacter(CharactersList[0]);
        }

        private void OnEnable()
        {
            recipesPanel.ReDrawRecipes(CurrentCharacter);
        }
    }
}