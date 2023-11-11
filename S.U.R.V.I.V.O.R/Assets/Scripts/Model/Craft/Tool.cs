using System;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

namespace TheRevenantsAge
{
    public enum ToolType
    {
        Hammer,
        Screwdiver,
        Saw,
        SharpItem,
        Wrench,
        
    }
    
    [System.Serializable]
    [RequireComponent(typeof(InventoryItem))]
    public class Tool : MonoBehaviour
    {
        private InventoryItem inventoryItem;
        public static ToolSprites GetSpriteByToolType(ToolType toolType)
        {
            var iconsDir = "Interface/Icons/ToolTypes/";
            switch (toolType)
            {
                case ToolType.Hammer:
                    return new ToolSprites(Resources.Load<Sprite>($"{iconsDir}hammer"),Resources.Load<Sprite>($"{iconsDir}hammerUnaviable") );
                case ToolType.Screwdiver:
                    return new ToolSprites(Resources.Load<Sprite>($"{iconsDir}screwdriver"),Resources.Load<Sprite>($"{iconsDir}screwdriverUnaviable") );
                case ToolType.Saw:
                    return new ToolSprites(Resources.Load<Sprite>($"{iconsDir}saw"),Resources.Load<Sprite>($"{iconsDir}sawUnaviable") );
                case ToolType.SharpItem:
                    return new ToolSprites(Resources.Load<Sprite>($"{iconsDir}sharp"),Resources.Load<Sprite>($"{iconsDir}sharpUnaviable") );
                case ToolType.Wrench:
                    return new ToolSprites(Resources.Load<Sprite>($"{iconsDir}wrench"),Resources.Load<Sprite>($"{iconsDir}wrenchUnaviable") );
                default: return new ToolSprites(Resources.Load<Sprite>($"{iconsDir}wrench"),Resources.Load<Sprite>($"{iconsDir}wrenchUnaviable") );
            }
        }

        [SerializeField] private List<ToolType> toolTypeList;

        public IEnumerable<ToolType> ToolTypeList => toolTypeList;

        private void Awake()
        {
            inventoryItem = GetComponent<InventoryItem>();
        }
    }

    public class ToolSprites
    {
        private Sprite aviableSprite;
        private Sprite unaviableSprite;

        public Sprite AviableSprite => aviableSprite;

        public Sprite UnaviableSprite => unaviableSprite;

        public ToolSprites(Sprite aviableSprite, Sprite unaviableSprite)
        {
            this.aviableSprite = aviableSprite;
            this.unaviableSprite = unaviableSprite;
        }
    }
}