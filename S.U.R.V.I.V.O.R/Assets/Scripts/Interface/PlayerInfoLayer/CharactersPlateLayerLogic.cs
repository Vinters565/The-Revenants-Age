using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheRevenantsAge;
using UnityEngine;

public class CharactersPlateLayerLogic : MonoBehaviour
{
    [SerializeField] private PlayerCharacteristicsPanel firstPlayerPanel;
    [SerializeField] private PlayerCharacteristicsPanel secondPlayerPanel;
    [SerializeField] private PlayerCharacteristicsPanel thirdPlayerPanel;
    [SerializeField] private PlayerCharacteristicsPanel fourthPlayerPanel;
    [SerializeField] private WalletDrawer walletDrawer;
    
    public void Init()
    {
        var playersArray = GlobalMapController.ChosenGroup.CurrentGroupMembers.ToArray();
        walletDrawer.CurrentGroup = GlobalMapController.ChosenGroup;
        firstPlayerPanel.Player = playersArray[0];
        secondPlayerPanel.Player = playersArray.Length > 1 ? playersArray[1] : null;
        thirdPlayerPanel.Player = playersArray.Length > 2 ? playersArray[2] : null;
        fourthPlayerPanel.Player = playersArray.Length > 3 ? playersArray[3] : null;
    }
}
