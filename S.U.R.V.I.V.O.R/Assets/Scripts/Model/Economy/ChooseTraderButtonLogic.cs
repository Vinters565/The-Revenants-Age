using System;
using Interface.Universal;
using TMPro;
using UnityEngine;

namespace TheRevenantsAge
{
    public class ChooseTraderButtonLogic : SwitchButton
    {
        private int id;
        private TradersPanelLogic tradersPanelLogic;
        [SerializeField] private TMP_Text traderName;//TODO сделать классом отображения кнопки, добавить метод ИНИТ с сигнатурой (Trader trader)
        
        public void InitReference(int id, TradersPanelLogic tradersPanelLogic, Trader trader)
        {
            this.id = id;
            this.tradersPanelLogic = tradersPanelLogic;
            layerSetActive.AddListener(OnButtonClick);
            traderName.text = trader.TraderName;
        }

        private void OnButtonClick()
        {
            tradersPanelLogic.OnButtonClick(id);
        }
    }
}