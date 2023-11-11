using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootAmountString : MonoBehaviour
{
    [SerializeField] private string lootTypeText;//патронов, медикаментов, материалов
    [SerializeField] private TextMeshProUGUI messageOfLootAmountText;
    [SerializeField] private Image backgroundImage;

    public void Redraw(float dropChance, List<Color> colorsToDraw)
    {
        if (dropChance == 0)
        {
            var color = colorsToDraw[0];
            messageOfLootAmountText.text = $"Нет {lootTypeText}";
            backgroundImage.color = new Color(color.r / 255,color.g / 255, color.b / 255,255);
        }
        else if (dropChance <= 0.1)
        {
            var color = colorsToDraw[1];
            messageOfLootAmountText.text = $"Мало {lootTypeText}"; 
            backgroundImage.color = new Color(color.r / 255,color.g / 255, color.b / 255,255);
        }
        else if (dropChance <= 0.20)
        {
            var color = colorsToDraw[2];
            messageOfLootAmountText.text = $"Достаточно {lootTypeText}"; 
            backgroundImage.color = new Color(color.r / 255,color.g / 255, color.b / 255,255);
        }
        else if (dropChance <= 0.40)
        {
            var color = colorsToDraw[3];
            messageOfLootAmountText.text = $"Много {lootTypeText}"; 
            backgroundImage.color = new Color(color.r / 255,color.g / 255, color.b / 255,255);
        }
        else
        {
            var color = colorsToDraw[4];
            messageOfLootAmountText.text = $"Очень много {lootTypeText}"; 
            backgroundImage.color = new Color(color.r / 255,color.g / 255, color.b / 255,255);
        }
        
    }
}
