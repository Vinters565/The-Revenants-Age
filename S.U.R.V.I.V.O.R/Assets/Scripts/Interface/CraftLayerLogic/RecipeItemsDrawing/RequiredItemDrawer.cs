using System.Collections;
using System.Collections.Generic;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.UI;

public class RequiredItemDrawer : MonoBehaviour
{
    [SerializeField] private Image ItemImage;
    [SerializeField] private Image AvailableImage;

    public void ReDraw(BaseItem baseItem, bool isAvailable)
    {
        ItemImage.sprite = baseItem.Data.Icon;
        AvailableImage.color = isAvailable ? Color.green : Color.red;
    }
}
