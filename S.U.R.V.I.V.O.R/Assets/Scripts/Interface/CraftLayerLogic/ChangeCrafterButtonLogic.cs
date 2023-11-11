using Interface.BodyIndicatorFolder;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace Interface.CraftLayerLogic
{
    public class ChangeCrafterButtonLogic : MonoBehaviour
    {
        [SerializeField] private BodyIndicator characterBodyIndicator;
        [SerializeField] private PlayerCharacteristicsPanel playerCharacteristicsPanel;
        [SerializeField] private Button button;
        [SerializeField] private CrafterCardLogic crafterCardLogic;
        [SerializeField] private bool isButtonUp;

        private IGlobalMapCharacter currentCharacter;
        
        public IGlobalMapCharacter CurrentCharacter
        {
            set
            {
                if (value == null)
                {
                    gameObject.SetActive(false);
                    return;
                }
                if (gameObject.activeInHierarchy)
                {
                    gameObject.SetActive(true);
                }
                characterBodyIndicator.Character = value;
                playerCharacteristicsPanel.Player = value;
                currentCharacter = value;
            }
        }
        
        public void Init()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            crafterCardLogic.ChangeCharactersOnGroupLayer(isButtonUp);
        }
    }
}