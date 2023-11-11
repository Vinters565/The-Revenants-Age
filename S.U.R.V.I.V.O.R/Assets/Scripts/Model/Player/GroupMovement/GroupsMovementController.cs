using UnityEngine;
using UnityEngine.UI;

namespace TheRevenantsAge
{
    public class GroupsMovementController : MonoBehaviour
    {
        public Button GroupMoveButton;

        public void Awake()
        {
            GroupMoveButton.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            GlobalMapController.ChosenGroup.GetComponent<GroupMovementLogic>().PreparingToMove();
        }
    }
}
