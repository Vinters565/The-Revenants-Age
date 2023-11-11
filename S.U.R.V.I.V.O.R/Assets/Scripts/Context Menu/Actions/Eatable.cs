using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Context_Menu;
using Inventory;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(ContextMenuItem))]
public class Eatable : MonoBehaviour, IContextMenuAction
{
    public string ButtonText { get; private set; }
    public bool Extendable { get; private set; }

    [field: SerializeField] public AudioClip SoundOnClick { get; }

    private EatableFood currentFood;

    private InventoryItem item;
    
    public void Awake()
    {
        ButtonText = "Употребить";
        Extendable = true;
        currentFood = GetComponent<EatableFood>();
        item = GetComponent<InventoryItem>();
    }


    public void OnButtonClickAction<T>(T value)
    {
        var character = value as IGlobalMapCharacter;
        character.Eat(currentFood);
    }

    public AudioClip GetSoundOnClick()
    {
        if (SoundOnClick != null)
            return SoundOnClick;
        return Sounds.GetContextActionSoundEffect(this, gameObject);
    }
}