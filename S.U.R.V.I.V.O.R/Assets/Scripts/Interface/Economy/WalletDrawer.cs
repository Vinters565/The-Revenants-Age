using System;
using System.Collections.Generic;
using TheRevenantsAge;
using TMPro;
using UnityEngine;

public class WalletDrawer : MonoBehaviour
{
    [SerializeField] private TMP_Text rublesAmount;
    [SerializeField] private TMP_Text dollarsAmount;

    private Dictionary<MoneyType, TMP_Text> currenciesText;
    private Group currentGroup;

    public Group CurrentGroup
    {
        get => currentGroup;
        set
        {
            if (currentGroup != null)
                currentGroup.MoneyAmountChanged -= ReDraw;
            currentGroup = value;
            if (currentGroup != null)
                currentGroup.MoneyAmountChanged += ReDraw;
        } 
    }

    private void Awake()
    {
        currenciesText = new Dictionary<MoneyType, TMP_Text>
        {
            { MoneyType.CenterRubles, rublesAmount },
            { MoneyType.PortDollar, dollarsAmount }
        };
    }

    private void ReDraw(Money money)
    {
        currenciesText[money.MoneyType].text = money.Amount.ToString();
    }

    private void ReDraw()
    {
        foreach (MoneyType value in Enum.GetValues(typeof(MoneyType)))
        {
            currenciesText[value].text = currentGroup.Currencies[value].Amount.ToString();
        }
    }
    
    private void OnEnable()
    {
        if(currentGroup != null)
            ReDraw();
    }
}
