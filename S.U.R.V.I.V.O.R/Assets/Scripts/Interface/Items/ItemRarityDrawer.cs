using UnityEngine;

namespace Interface.Items
{
    public static class ItemRarityDrawer
    {
        private const int ALPHA_CHANNEL = 255;

        public static Color32 GetColorByRarity(ItemType rarity)
        {
            switch (rarity)
            {
                case ItemType.Scrap:
                    return new Color32(158, 158, 171,ALPHA_CHANNEL);
                case ItemType.Medicine:
                    return new Color32(231, 2, 2,ALPHA_CHANNEL);
                case ItemType.Food:
                    return new Color32(129, 178, 20,ALPHA_CHANNEL);
                case ItemType.AmmoContainers:
                    return new Color32(249, 187, 2,ALPHA_CHANNEL);
                case ItemType.GunModules:
                    return new Color32(49, 36, 164,ALPHA_CHANNEL);
                case ItemType.Clothes:
                    return new Color32(136, 199, 180,ALPHA_CHANNEL);
                case ItemType.GunMass:
                    return new Color32(15, 82, 186,ALPHA_CHANNEL);
                case ItemType.GunSimple:
                    return new Color32(92, 0, 156,ALPHA_CHANNEL);
                case ItemType.GunRare:
                    return new Color32(245, 46, 99,ALPHA_CHANNEL);
                case ItemType.GunEpic:
                    return new Color32(252, 84, 4,ALPHA_CHANNEL);
                default:
                    return new Color32(255, 255, 255, ALPHA_CHANNEL);
            }
        }
    }
}