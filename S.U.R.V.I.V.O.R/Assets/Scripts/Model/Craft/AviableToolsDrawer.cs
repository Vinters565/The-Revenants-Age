using System;
using System.Collections;
using System.Collections.Generic;
using TheRevenantsAge;
using UnityEngine;

public class AviableToolsDrawer : MonoBehaviour
{
    [SerializeField] private ToolAviabilityDrawer toolAviabilityPrefab;
    private Dictionary<ToolType, ToolAviabilityDrawer> toolAviabilityObjects;

    public void Init()
    {
        toolAviabilityObjects = new Dictionary<ToolType, ToolAviabilityDrawer>
        {
            { ToolType.Hammer , GenerateAviabilityObject(ToolType.Hammer)},
            { ToolType.Wrench , GenerateAviabilityObject(ToolType.Wrench)},
            { ToolType.Saw , GenerateAviabilityObject(ToolType.Saw)},
            { ToolType.Screwdiver , GenerateAviabilityObject(ToolType.Screwdiver)},
            { ToolType.SharpItem , GenerateAviabilityObject(ToolType.SharpItem)}
        };
    }

    public void ReDrawAllTools(List<ToolType> toolTypes)
    {
        foreach (var pair in toolAviabilityObjects)
        {
            pair.Value.ReDrawAviability(toolTypes.Contains(pair.Key));
        }
    }

    private ToolAviabilityDrawer GenerateAviabilityObject(ToolType toolType)
    {
        var sprites = Tool.GetSpriteByToolType(toolType);
        var drawer = Instantiate(toolAviabilityPrefab, transform);
        drawer.AviablePanel.sprite = sprites.AviableSprite;
        drawer.UnaviablePanel.sprite = sprites.UnaviableSprite;
        drawer.AviablePanel.gameObject.SetActive(true);
        drawer.UnaviablePanel.gameObject.SetActive(false);
        return drawer;
    }

    private void OnEnable()
    {

    }
}
