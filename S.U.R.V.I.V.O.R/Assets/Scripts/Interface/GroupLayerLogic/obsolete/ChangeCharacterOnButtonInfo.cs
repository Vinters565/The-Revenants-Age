using System;
using Interface.BodyIndicatorFolder;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.GroupLayerLogic
{
    [Obsolete]
    public class ChangeCharacterOnButtonInfo : MonoBehaviour
    {
        [SerializeField] private bool isButtonOnLeftCart;
        [SerializeField] private bool isButtonUpper;
        [SerializeField] private GroupLayerLogic groupLayerLogic;
        [SerializeField] private BodyIndicator characterBodyIndicator;
        [SerializeField] private PlayerCharacteristicsPanel playerCharacteristicsPanel;
        public Button button;

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

        public virtual void Init()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            groupLayerLogic.ChangeCharactersOnGroupLayer(isButtonOnLeftCart, isButtonUpper);
        }
    }
}
