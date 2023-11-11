using System.Collections.Generic;
using Audio;
using Inventory;
using Inventory.SpecialCells;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SpecialGunCell : SpecialCell
{
    [SerializeField] private GunType type;
    [SerializeField] private GunMagazineSpecialCell magazineSlot;
    [SerializeField] private SpecialGunModuleCell springSlot;
    [SerializeField] private SpecialGunModuleCell shutterSlot;
    [SerializeField] private SpecialGunModuleCell scopeSlot;
    [SerializeField] private SpecialGunModuleCell gripSlot;
    [SerializeField] private SpecialGunModuleCell tacticalSlot;
    [SerializeField] private SpecialGunModuleCell supressorSlot;

    private List<SpecialGunModuleCell> allCells;

    private IGlobalMapCharacter currentCharacter;

    public IGlobalMapCharacter CurrentCharacter
    {
        get => currentCharacter;
        set
        {
            if (currentCharacter != null)
                currentCharacter.GunsChanged -= GunChanged;
            currentCharacter = value;
            if (currentCharacter != null && isActiveAndEnabled)
                currentCharacter.GunsChanged += GunChanged;
            Init();
        }
    }

    public override void Init()
    {
        base.Init();
        allCells = new List<SpecialGunModuleCell>
        {
            springSlot,
            shutterSlot,
            scopeSlot,
            gripSlot,
            tacticalSlot,
            supressorSlot
        };
        magazineSlot.Init();
        foreach (var cell in allCells)
        {
            cell.Init();
        }
    }

    protected override void PlaceItem(InventoryItem item)
    {
        base.PlaceItem(item);
        if (item.Data.Size.y > item.Data.Size.x && !item.IsRotated)
        {
            item.Rotate();
        }
        item.BaseItem.ItemOwner = CurrentCharacter;
        ChangeCharacterGuns(item.GetComponent<Gun>());
        InventoryController.SelectedItem = null;

        AudioManager.Instance.PlayOneShotSFX(item.OnPlaceAudioClip);
    }

    public override void CheckNewItem(InventoryItem item)
    {
        if (item == null || item == PlacedItem)
        {
            ReDraw();
        }
        else if (item != PlacedItem && PlacedItem != null)
        {
            PlacedItem.gameObject.SetActive(false);
            PlacedItem = item;
            item.gameObject.SetActive(true);
            ReDraw();
        }
        else if (item != PlacedItem && PlacedItem == null)
        {
            PlacedItem = item;
            PlacedItem.gameObject.SetActive(true);
            ReDraw();
        }

        var gun = GetGun();

        foreach (var cell in allCells)
        {
            cell.CheckNewItem(gun && gun.GunModules[cell.type]
                ? gun.GunModules[cell.type].GetComponent<InventoryItem>()
                : null);
        }
    }

    protected override void GiveItem()
    {
        base.GiveItem();
        PlaceNullItem();
        ChangeCharacterGuns(null);
    }

    public override void UpdateItem(InventoryItem item)
    {
        base.UpdateItem(item);
        PlaceAllModules();
    }

    protected override bool CanInsertIntoSlot(InventoryItem item)
    {
        return PlacedItem == null && item && item.GetComponent<Gun>() && item.GetComponent<Gun>().Data.GunType == type;
    }

    private void ChangeCharacterGuns(Gun gun)
    {
        switch (type)
        {
            case GunType.PrimaryGun:
                CurrentCharacter.PrimaryGun = gun;
                break;
            case GunType.SecondaryGun:
                CurrentCharacter.SecondaryGun = gun;
                break;
        }
    }

    public Gun GetGun()
    {
        if (CurrentCharacter == null)
            return null;
        switch (type)
        {
            case GunType.PrimaryGun:
                return CurrentCharacter.PrimaryGun;

            case GunType.SecondaryGun:
                return CurrentCharacter.SecondaryGun;
        }

        return null;
    }

    private void OnModuleChanged(GunModuleType type)
    {
        var gun = GetGun();
        if (gun == null) return;

        if (type == GunModuleType.Magazine)
        {
            var x = gun.CurrentMagazine ? gun.CurrentMagazine.GetComponent<InventoryItem>() : null;
            magazineSlot.CheckNewItem(x);
            return;
        }

        SpecialGunModuleCell cell;
        switch (type)
        {
            case GunModuleType.Scope:
                cell = scopeSlot;
                break;
            case GunModuleType.Shutter:
                cell = shutterSlot;
                break;
            case GunModuleType.Spring:
                cell = springSlot;
                break;
            case GunModuleType.Suppressor:
                cell = supressorSlot;
                break;
            case GunModuleType.Grip:
                cell = gripSlot;
                break;
            case GunModuleType.Tactical:
                cell = tacticalSlot;
                break;
            default:
                cell = null;
                break;
        }

        if (cell != null)
        {
            cell.CheckNewItem(gun.GunModules[type] ? gun.GunModules[type].GetComponent<InventoryItem>() : null);
            return;
        }

        foreach (var specialGunModuleCell in allCells)
        {
            specialGunModuleCell.CheckNewItem(gun.GunModules[type] != null
                ? gun.GunModules[specialGunModuleCell.type].GetComponent<InventoryItem>()
                : null);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        var gun = GetGun();
        if (CurrentCharacter != null)
            CurrentCharacter.GunsChanged -= GunChanged;
        if (gun == null) return;
        gun.ModulesChanged -= OnModuleChanged;
        InventoryController.Instance.SelectedItemChanged -= OnSelectedItemChanged;
    }

    protected override void OnEnable()
    {
        if (CurrentCharacter == null) return;
        base.OnEnable();
        PlaceAllModules();
        var gun = GetGun();
        if (CurrentCharacter != null)
            CurrentCharacter.GunsChanged += GunChanged;
        if (gun == null) return;
        gun.ModulesChanged += OnModuleChanged;
    }

    private void PlaceAllModules()
    {
        var gun = GetGun();
        if (gun == null)
        {
            foreach (var cell in allCells)
            {
                cell.UpdateItem(null);
            }

            magazineSlot.UpdateItem(null);
        }
        else
        {
            magazineSlot.UpdateItem(gun.CurrentMagazine != null
                ? gun.CurrentMagazine.GetComponent<InventoryItem>()
                : null);
            foreach (var cell in allCells)
            {
                var type = cell.type;
                cell.UpdateItem(
                    gun.GunModules[type] != null ? gun.GunModules[type].GetComponent<InventoryItem>() : null);
            }
        }
    }

    private void GunChanged(Gun gun, GunType gType)
    {
        if (gType != type) return;
        if (gun != null)
            gun.ModulesChanged -= OnModuleChanged;
        var newGun = GetGun();
        foreach (var cell in allCells)
        {
            cell.CurrentGun = newGun;
        }
        if (newGun != null)
        {
            newGun.ModulesChanged += OnModuleChanged;
            CheckNewItem(newGun.GetComponent<InventoryItem>());
            PlaceAllModules();
            return;
        }

        CheckNewItem(null);
        PlaceAllModules();
    }
}