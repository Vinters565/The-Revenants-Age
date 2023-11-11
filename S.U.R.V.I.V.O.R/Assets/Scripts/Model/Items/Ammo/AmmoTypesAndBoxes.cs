using System.Collections.Generic;
using UnityEngine;

namespace TheRevenantsAge
{
    public static class AmmoTypesAndBoxes
    {
        public static readonly Dictionary<SingleAmmo, GameObject> AmmoBoxDictionary = new()
        {
            {Resources.Load<SingleAmmo>("Items/Weapons/Ak-74M/Magazine/762_Ammo"), Resources.Load<GameObject>("Items/Ammo/C762/C762Box")},
            {Resources.Load<SingleAmmo>("Items/Weapons/Makarov/Magazine/C9x18"), Resources.Load<GameObject>("Items/Ammo/C9x18/C9x18BoxTest")},
        };
    }
}
