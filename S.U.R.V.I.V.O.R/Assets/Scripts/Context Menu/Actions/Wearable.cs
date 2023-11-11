using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Context_Menu;
using Inventory;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ContextMenuItem))]
public class Wearable : MonoBehaviour, IContextMenuAction
{
    public string ButtonText { get; private set; }

    public bool Extendable { get; private set; }

    public AudioClip SoundOnClick { get; }

    private Clothes currentClothes;

    private InventoryItem item;


    public void Awake()
    {
        ButtonText = "Надеть";
        Extendable = true;
        currentClothes = GetComponent<Clothes>();
        item = GetComponent<InventoryItem>();
    }


    public void OnButtonClickAction<T>(T value)
    {
        item.gameObject.SetActive(false);
        var inventory = item.InventoryGrid;
        var character = value as IGlobalMapCharacter;
        inventory.PickUpItem(item);
        var isSuccessful = character.ManBody.Wear(currentClothes);

        if (!isSuccessful)
        {
            inventory.InsertItem(item);
            item.gameObject.SetActive(true);
        }
        else
            AudioManager.Instance.PlayOneShotSFX(item.OnPlaceAudioClip);
    }
}