using System;
using System.Collections;
using System.Collections.Generic;
using Interface.CraftLayerLogic;
using Interface.Items;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TooltipPartDrawer : MonoBehaviour
{
    private static string tooltipPartPrefabPath = "Interface/Prefabs/Tooltip/TooltipPart";
    
    private static string textPlainStringPrefab = "Interface/Prefabs/Tooltip/TextPlainStringPrefab";
    private static string textMainStringPrefab = "Interface/Prefabs/Tooltip/TextMainStringPrefab";
    private static string stringWithImagePrefab = "Interface/Prefabs/Tooltip/StringWithImage";
    
    
    public static TooltipPartDrawer InitPart()
    {
        return Instantiate(Resources.Load(tooltipPartPrefabPath)).GetComponent<TooltipPartDrawer>();
    }

    public void AddMainText(string text)
    {
        var textBox = Instantiate(Resources.Load(textMainStringPrefab), transform);
        var tmp = textBox.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = true;
    }

    
    public void AddPlainText(string text)
    {
        var textBox = Instantiate(Resources.Load(textPlainStringPrefab), transform);
        var tmp = textBox.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
    }

    public void AddImageWithText(string text, Sprite image)
    {
        var imageBox = Instantiate(Resources.Load(stringWithImagePrefab), transform);
        var drawer = imageBox.GetComponent<ImageWithTextDrawer>();
        drawer.ReDrawItem(image,text);
    }
}
