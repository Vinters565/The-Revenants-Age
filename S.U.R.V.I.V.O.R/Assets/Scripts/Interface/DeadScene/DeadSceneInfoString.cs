using System.Collections;
using System.Collections.Generic;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeadSceneInfoString : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amount;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI characteristicName;


    public void Init(StatisticCharacteristic characteristic)
    {
        amount.text = characteristic.Amount;
        icon.sprite = characteristic.Icon;
        characteristicName.text = characteristic.CharacteristicName;
    }
}