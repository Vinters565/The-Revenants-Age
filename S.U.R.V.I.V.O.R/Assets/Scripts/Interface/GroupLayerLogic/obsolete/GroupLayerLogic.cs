using System;
using System.Collections.Generic;
using System.Linq;
using TheRevenantsAge;
using UnityEngine;

namespace Interface.GroupLayerLogic
{
    [Obsolete]
    public class PlayerCartReDrawInfo
    {
        public IGlobalMapCharacter previousCharacter;
        public IGlobalMapCharacter currentCharacter;
        public IGlobalMapCharacter nextCharacter;

        public PlayerCartReDrawInfo(IGlobalMapCharacter previous, IGlobalMapCharacter current, IGlobalMapCharacter next)
        {
            nextCharacter = next;
            currentCharacter = current;
            previousCharacter = previous;
        }

        public void Clear()
        {
            previousCharacter = null;
            currentCharacter = null;
            nextCharacter = null;
        }
    }
    [Obsolete]
    public class GroupLayerLogic : MonoBehaviour
    {

        [SerializeField] private PlayerCartLogic leftCart;

        [SerializeField] private PlayerCartLogic rightCart;
    
        [SerializeField] private List<IGlobalMapCharacter> charactersList;

        private PlayerCartReDrawInfo leftCartReDrawInfo;
        private PlayerCartReDrawInfo rightCartReDrawInfo;
    
        public void ChangeCharactersOnGroupLayer(bool isSwitchOnLeftCart, bool isSwitchOnUpCharacter)
        {
            SwitchPointers(isSwitchOnLeftCart ? leftCartReDrawInfo : rightCartReDrawInfo,
                !isSwitchOnLeftCart ? leftCartReDrawInfo : rightCartReDrawInfo,
                isSwitchOnUpCharacter);

            leftCart.ReDraw(leftCartReDrawInfo);
            rightCart.ReDraw(rightCartReDrawInfo);
        }

        private void SwitchPointers(PlayerCartReDrawInfo cartToSwitch, PlayerCartReDrawInfo cartToRedraw, bool isSearchForward)
        {
            if (isSearchForward)
            {
                SwitchPoints(ref cartToSwitch.nextCharacter,
                    ref cartToSwitch.currentCharacter, 
                    ref cartToSwitch.previousCharacter,
                    true);
            }
            else
            {
                SwitchPoints(ref cartToSwitch.previousCharacter, 
                    ref cartToSwitch.currentCharacter,
                    ref cartToSwitch.nextCharacter, 
                    true);
            }
            ReDrawCart(cartToRedraw);

            void ReDrawCart(PlayerCartReDrawInfo cart)
            {
                var index = charactersList.IndexOf(cart.currentCharacter);
                cart.previousCharacter = FindCharacter(index, false);
                cart.nextCharacter = FindCharacter(index, true);
            }
        
            void SwitchPoints(ref IGlobalMapCharacter characterBehind, ref IGlobalMapCharacter characterCurrent,
                ref IGlobalMapCharacter characterForward, bool isSearchForward)
            {
                var characterToChange = FindCharacter(charactersList.IndexOf(characterCurrent), isSearchForward);
                if (characterToChange == null)//Оба персонажа демонстрируются
                {
                    characterBehind = characterCurrent;
                    characterForward = characterCurrent;
                    return;
                }
                characterCurrent = characterToChange;
                var characterToChangeForward = FindCharacter(charactersList.IndexOf(characterCurrent), !isSearchForward);
                var characterToChangeBehind = FindCharacter(charactersList.IndexOf(characterCurrent), isSearchForward);
                characterBehind = characterToChangeBehind == null ? characterCurrent : characterToChangeBehind;
                characterForward = characterToChangeForward == null ? characterCurrent : characterToChangeForward;
            }
        
            IGlobalMapCharacter FindCharacter(int index, bool isSearchForward)
            {
                if (index == -1)
                    return null;
                var iter = index;
                if (charactersList.Count == 1)
                    return charactersList.FirstOrDefault();
                while (true)
                {
                    var lenght = charactersList.Count;
                    if (lenght == 2 && leftCartReDrawInfo.currentCharacter != rightCartReDrawInfo.currentCharacter)
                        return charactersList[index];
                    var currentCharacter = charactersList[iter];
                    if (leftCartReDrawInfo.currentCharacter != currentCharacter &&
                        rightCartReDrawInfo.currentCharacter != currentCharacter)
                        return currentCharacter;
                    iter = !isSearchForward ? iter - 1 : iter + 1;

                    if (iter == index) 
                        return null;
                    if (iter == -1) 
                        iter = lenght - 1;
                    if (iter == lenght) 
                        iter = 0;
                }
            }
        }

        public void Init()
        {
            charactersList = TheRevenantsAge.GlobalMapController.ChosenGroup.CurrentGroupMembers.ToList();
            leftCartReDrawInfo = new PlayerCartReDrawInfo(null, null, null);
            rightCartReDrawInfo = new PlayerCartReDrawInfo(null, null, null);
            ReCalculateCartsInfo();
            leftCart.Init(leftCartReDrawInfo);
            rightCart.Init(rightCartReDrawInfo);
        }

        private void ReCalculateCartsInfo()
        {
            leftCartReDrawInfo.currentCharacter = charactersList[0];
            SwitchPointers(leftCartReDrawInfo,rightCartReDrawInfo,true);
            if (charactersList.Count > 1)
            {
                rightCart.gameObject.SetActive(true);
                rightCartReDrawInfo.currentCharacter = charactersList[1];
                SwitchPointers(rightCartReDrawInfo,leftCartReDrawInfo,true);
            }
            else
            {
                rightCart.DoSomethingIfYouAreNotNeeded();
            }
        }
    
        private void OnGroupChanged(Group oldGroup, Group newGroup)
        {
            charactersList = newGroup.CurrentGroupMembers.ToList();
            ReCalculateCartsInfo();
        }
        
        private void OnEnable()
        {
            TheRevenantsAge.GlobalMapController.ChosenGroupChange += OnGroupChanged;
        }

        private void OnDisable()
        {
            TheRevenantsAge.GlobalMapController.ChosenGroupChange -= OnGroupChanged;
        }
    }
}