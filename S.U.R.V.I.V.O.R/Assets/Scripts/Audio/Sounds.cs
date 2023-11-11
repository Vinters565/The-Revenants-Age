using System;
using System.Collections.Generic;
using System.Reflection;
using Context_Menu;
using Context_Menu.Actions;
using UnityEngine;

namespace Audio
{
    public static class Sounds
    {
        private static readonly Dictionary<UISoundEffectsType, string> uiSoundEffectsDictionary = new()
        {
            {
                UISoundEffectsType.DefaultButton, "Audio/SFX/Button-Click"
            },
            {
                UISoundEffectsType.LoadScene, "Audio/SFX/Load-Scene"
            },
            {
                UISoundEffectsType.UnloadScene, "Audio/SFX/Unload-Scene"
            },
        };

        private static readonly Dictionary<InventoryItemSoundsType, InventoryItemSounds> itemSoundsDictionary = new()
        {
            {
                InventoryItemSoundsType.Basic,
                new("Audio/SFX/Default-Down", "Audio/SFX/Default-Up")
            },
            {
                InventoryItemSoundsType.Weapon,
                new("Audio/SFX/Gun-Down", "Audio/SFX/Gun-Up")
            },
            {
                InventoryItemSoundsType.Cloth,
                new("Audio/SFX/Clothes-Down", "Audio/SFX/Clothes-Up")
            },
            {
                InventoryItemSoundsType.Medicine,
                new("Audio/SFX/Med-Down", "Audio/SFX/Med-Up")
            },
            {
                InventoryItemSoundsType.Ammo,
                new("Audio/SFX/Ammo-Down", "Audio/SFX/Ammo-Up")
            },
        };

        public static AudioClip GetAudioOnPlaceItemByType(InventoryItemSoundsType type) =>
            Resources.Load<AudioClip>(itemSoundsDictionary[type].OnPlaceItemAudioClip);

        public static AudioClip GetAudioOnPickUpItemByType(InventoryItemSoundsType type) =>
            Resources.Load<AudioClip>(itemSoundsDictionary[type].OnPickUpItemAudioClip);

        public static AudioClip GetUISoundEffect(UISoundEffectsType type) =>
            Resources.Load<AudioClip>(uiSoundEffectsDictionary[type]);

        public static AudioClip GetContextActionSoundEffect(IContextMenuAction type, GameObject item)
        {
            if (type.GetType().IsSubclassOf(typeof(Healable)))
            {
                return Resources.Load<AudioClip>("Audio/SFX/Button-Click");
            }

            if (type.GetType() == typeof(Conserved))
            {
                return Resources.Load<AudioClip>("Audio/SFX/Button-Click");
            }

            if (type.GetType() == typeof(Cookable))
            {
                return Resources.Load<AudioClip>("Audio/SFX/Button-Click");
            }

            if (type.GetType() == typeof(DetailedMagazine))
            {
                return Resources.Load<AudioClip>("Audio/SFX/Button-Click");
            }

            if (type.GetType() == typeof(Eatable))
            {
                if (item != null)
                {
                    var eatableFood = item.GetComponent<EatableFood>();
                    if (eatableFood != null)
                        return eatableFood.Data.DeltaWater > eatableFood.Data.DeltaHunger
                            ? Resources.Load<AudioClip>("Audio/SFX/Drinking")
                            : Resources.Load<AudioClip>("Audio/SFX/Eating");
                }
            }

            if (type.GetType() == typeof(Equipable))
            {
                return Resources.Load<AudioClip>("Audio/SFX/Button-Click");
            }

            if (type.GetType() == typeof(Reloadable))
            {
                return Resources.Load<AudioClip>("Audio/SFX/Button-Click");
            }

            if (type.GetType() == typeof(Salvagable))
            {
                return Resources.Load<AudioClip>("Audio/SFX/Button-Click");
            }

            if (type.GetType() == typeof(Unpackable))
            {
                return Resources.Load<AudioClip>("Audio/SFX/Button-Click");
            }

            if (type.GetType() == typeof(Wearable))
            {
                return Resources.Load<AudioClip>("Audio/SFX/Button-Click");
            }

            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}