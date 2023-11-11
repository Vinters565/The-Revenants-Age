using UnityEngine;

namespace Audio
{
    public class InventoryItemSounds
    {
        public string OnPlaceItemAudioClip { get; }
        public string OnPickUpItemAudioClip { get; }

        public InventoryItemSounds(string onPlaceItemAudioClip, string onPickUpItemAudioClip)
        {
            OnPlaceItemAudioClip = onPlaceItemAudioClip;
            OnPickUpItemAudioClip = onPickUpItemAudioClip;
        }
    }
}
