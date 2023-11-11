using System;
using UnityEngine;

public class NestedTooltipItem
{
    public Sprite icon;
    public string itemName;
    public float value;

    public NestedTooltipItem(string itemName, Sprite icon, float value = default)
    {
        this.itemName = itemName;
        this.icon = icon;
        if (Math.Abs(value - default(float)) > Mathf.Epsilon)
            this.value = value;
    }
}
