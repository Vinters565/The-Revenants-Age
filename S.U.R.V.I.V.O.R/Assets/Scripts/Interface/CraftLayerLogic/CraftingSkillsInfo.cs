using Interface.PlayerInfoLayer;
using TheRevenantsAge;
using UnityEngine;

namespace Interface.CraftLayerLogic
{
    public class CraftingSkillsInfo : MonoBehaviour
    {
        [SerializeField] private SkillInfo craftInfo;
        [SerializeField] private SkillInfo cookingInfo;
        [SerializeField] private SkillInfo firstAidInfo;
        
        public ICharacter CurrentCharacter { get; set; }
        
        public void Init(ICharacter character)
        {
            if (character == null)
            {
                craftInfo.Init(1);
                cookingInfo.Init(1);
                firstAidInfo.Init(1);
            }
            else
            {
                craftInfo.Init(character.Skills.Crafting.CurrentLevel + 1);
                cookingInfo.Init(character.Skills.Cooking.CurrentLevel + 1);
                firstAidInfo.Init(character.Skills.Healing.CurrentLevel + 1);
            }

            CurrentCharacter = character;
        }
    
        public void OnEnable()
        {
            if (CurrentCharacter == null) return;
            craftInfo.SetValue(CurrentCharacter.Skills.Crafting);
            cookingInfo.SetValue(CurrentCharacter.Skills.Cooking);
            firstAidInfo.SetValue(CurrentCharacter.Skills.Healing);
        }
    }
}