using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using TheRevenantsAge;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(InventoryItem))]
public class InventoryItemEditor : Editor
{
    private InventoryItem Target => (InventoryItem)target;
    private void CreateImage()
    {
        var image = Instantiate(Resources.Load("Items/Background/ItemImage"),Target.transform).GetComponent<Image>();
        image.gameObject.name = "ItemImage";
        var Data = Target.GetComponent<BaseItem>().Data;
        image.sprite = Data.Icon;
        var size = (Vector2) Data.Size * InventoryGrid.TileSize;
        image.GetComponent<RectTransform>().sizeDelta = size;
        EditorUtility.SetDirty(Target.gameObject);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(5);
        if (GUILayout.Button("CreateImage"))
        {
            CreateImage();
        }
        if (GUILayout.Button("CreateBackground"))
        {
            CreateBackground();
        }
        if (GUILayout.Button("CreateAll"))
        {
            CreateBackground();
            CreateImage();
        }
    }
    private void CreateBackground()
    {
        var image = Instantiate(Resources.Load("Items/Background/ItemBackground"),Target.transform);
        image.name = "ItemBackground";
        EditorUtility.SetDirty(Target.gameObject);
    }
}
