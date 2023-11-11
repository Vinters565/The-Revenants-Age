using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using JetBrains.Annotations;
using TheRevenantsAge;
using UnityEngine;

namespace Context_Menu
{
    public interface IContextMenuAction
    {
        public string ButtonText { get; }
    
        public bool Extendable { get; }
        
        public AudioClip SoundOnClick { get; }

        public void OnButtonClickAction<T>([CanBeNull] T value);

        public virtual IEnumerable GetValues()
        {
            var result = new List<Tuple<IGlobalMapCharacter, string>>();
            foreach (var character in TheRevenantsAge.GlobalMapController.ChosenGroup.CurrentGroupMembers)
            {
                result.Add(new Tuple<IGlobalMapCharacter, string>(character, $"{character.FirstName} {character.SurName}"));
            }
            return result;
        }

        public virtual AudioClip GetSoundOnClick()
        {
            if (SoundOnClick != null)
                return SoundOnClick;
            return Sounds.GetContextActionSoundEffect(this, null);
        }
    }
}
