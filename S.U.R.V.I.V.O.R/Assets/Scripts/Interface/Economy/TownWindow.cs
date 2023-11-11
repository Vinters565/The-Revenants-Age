using System.Collections.Generic;
using Interface.Economy;
using TheRevenantsAge;
using UnityEngine;

public class TownWindow : MonoBehaviour
{
    //TODO Создать класс TradeWindow и вынести логику туда
    [SerializeField] private TradersPanelLogic tradersPanelLogic;
    [SerializeField] private BuyList buyList;
    public static TownWindow Instance { get; private set; }

    private Town currentTown;

    public Town CurrentTown
    {
        get => currentTown;
        set
        {
            currentTown = value;
            ReDraw();
        }
    }

    public BuyList BuyList => buyList;

    public Trader CurrentTrader => tradersPanelLogic.ChosenTrader;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            Debug.LogError("Town Window is not singleton");
        }
    }

    public void Open(Town town)
    {
        if (town != currentTown)
            CurrentTown = town;
        gameObject.SetActive(true);
    }

    private void ReDraw()
    {
        SetToDefaults();
        ReDrawTradersPanel();
    }

    private void ReDrawTradersPanel()
    {
        tradersPanelLogic.ReDraw(currentTown.Traders);
    }
    
    private void SetToDefaults()
    {
        tradersPanelLogic.ReDraw(new List<Trader>());
    }
}
