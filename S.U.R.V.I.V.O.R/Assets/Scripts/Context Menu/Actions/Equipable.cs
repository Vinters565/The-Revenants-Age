using Audio;
using Context_Menu;
using Inventory;
using Inventory.SpecialCells;
using TheRevenantsAge;
using UnityEngine;

[RequireComponent(typeof(ContextMenuItem))]
public class Equipable : MonoBehaviour, IContextMenuAction
{
    public string ButtonText { get; private set; }

    public bool Extendable { get; private set; }

    public AudioClip SoundOnClick { get; }

    private Gun currentGun;

    private InventoryItem item;


    public void Awake()
    {
        ButtonText = "Экипировать";
        Extendable = true;
        currentGun = GetComponent<Gun>();
        item = GetComponent<InventoryItem>();
    }


    public void OnButtonClickAction<T>(T value)
    {
        currentGun.gameObject.SetActive(false);
        var inventory = item.InventoryGrid;
        var character = value as IGlobalMapCharacter;
        inventory.PickUpItem(item);
        switch (currentGun.Data.GunType)
        {
            case GunType.PrimaryGun:
                character.PrimaryGun = currentGun;
                break;
            case GunType.SecondaryGun:
                character.SecondaryGun = currentGun;
                break;
        }
    }
}