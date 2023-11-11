using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using TheRevenantsAge;
using TMPro;
using UnityEngine;

public class MagazineWindow : MonoBehaviour
{
    private static MagazineWindow instance;

    public static MagazineWindow Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MagazineWindow>();
                instance.Init();
            }

            return instance;
        }
    }

    private TextMeshProUGUI itemName;
    private RectTransform backgroundRectTransform;
    private RectTransform magazineСontainer;
    private RectTransform ammoPref;
    private RectTransform blankPref;
    private List<GameObject> allObjects;
    private Magazine currentMagazine;
    private RectTransform dischargeMenu;
    private QuantitySettings dischargeQuantitySettings;
    private RectTransform chargeMenu;
    private QuantitySettings chargeQuantitySettings;
    private TMP_Dropdown chargeSelectTypeAmmo;
    private List<SingleAmmo> allTypesAmmo;
    private TMP_Dropdown chargeSelectAmmoBox;
    private AmmoBox selectedAmmoBox;
    private List<AmmoBox> ammoBoxesByTypeAmmo;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (instance == null)
        {
            instance = this;
            Init();
        }
    }
    
    private void Init()
    {
        backgroundRectTransform = transform.Find("Background").GetComponent<RectTransform>();
        magazineСontainer = backgroundRectTransform.Find("ScrollArea").Find("Magazine").GetComponent<RectTransform>();
        ammoPref = magazineСontainer.Find("Ammo").GetComponent<RectTransform>();
        blankPref = magazineСontainer.Find("Blank").GetComponent<RectTransform>();
        allObjects = new List<GameObject>();
        currentMagazine = null;
        dischargeMenu = backgroundRectTransform.Find("Discharge").GetComponent<RectTransform>();
        dischargeQuantitySettings = dischargeMenu.Find("QuantitySettings").GetComponent<QuantitySettings>();
        chargeMenu = backgroundRectTransform.Find("Charge").GetComponent<RectTransform>();
        chargeQuantitySettings = chargeMenu.Find("QuantitySettings").GetComponent<QuantitySettings>();
        chargeSelectTypeAmmo = chargeMenu.Find("AmmoTypeSelection").Find("Dropdown").GetComponent<TMP_Dropdown>();
        chargeSelectTypeAmmo.onValueChanged.AddListener(OnChangeAmmoType);
        allTypesAmmo = new List<SingleAmmo>();
        chargeSelectAmmoBox = chargeMenu.Find("AmmoBoxSelection").Find("Dropdown").GetComponent<TMP_Dropdown>();
        chargeSelectAmmoBox.onValueChanged.AddListener(OnChangeAmmoBox);
        ammoBoxesByTypeAmmo = new List<AmmoBox>();
        selectedAmmoBox = null;
        itemName = backgroundRectTransform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        HideWindow();
    }

    public void ShowWindow(Magazine magazine)
    {
        HideWindow();
        currentMagazine = magazine;
        RedrawMagazineContent();

        dischargeQuantitySettings.MaxValue = magazine.CurrentNumberAmmo;

        UpdateAmmoTypeSelection();

        chargeSelectAmmoBox.interactable = false;
        chargeQuantitySettings.SetInteractable(false);

        backgroundRectTransform.gameObject.SetActive(true);
    }

    public void HideWindow()
    {
        backgroundRectTransform.gameObject.SetActive(false);
        ClearAllAmmo();

        currentMagazine = null;
        dischargeQuantitySettings.ResetValue();
        chargeSelectTypeAmmo.value = 0;
        chargeSelectTypeAmmo.onValueChanged?.Invoke(0);
    }

    public void OnDischargeButtonClick()
    {
        DischargeAmmo();
        UpdateAmmoTypeSelection();
        UpdateAmmoBoxSelection();
        UpdateDischargeQuantitySettings();
        RedrawMagazineContent();
    }

    public void OnChargeButtonClick()
    {
        ChargeAmmo();
        UpdateAmmoTypeSelection();
        UpdateAmmoBoxSelection();
        UpdateDischargeQuantitySettings();
        MagazineFullCheck();
        RedrawMagazineContent();
    }

    private void RedrawMagazineContent()
    {
        if (currentMagazine == null) return;
        itemName.text = currentMagazine.gameObject.GetComponent<BaseItem>().Data.ItemName;
        ClearAllAmmo();

        var ammoStack = currentMagazine.GetAmmoStack.ToArray();
        for (int i = 0; i < ammoStack.Length; i++)
        {
            var ammoObj = Instantiate(ammoPref, magazineСontainer);
            ammoObj.gameObject.SetActive(true);
            ammoObj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"{ammoStack[i].Caliber.ToString()} - патрон {i+1}";
            //ammoObj.GetComponent<Image>().color = i % 2 == 0 ? new Color(1, 1, 1, 1) : new Color(0.75f, 0.75f, 0.75f, 1); Пока уберу
            allObjects.Add(ammoObj.gameObject);
        }
        
        for (int i = 0; i < currentMagazine.MaxAmmoAmount - currentMagazine.CurrentNumberAmmo; i++)
        {
            var blank = Instantiate(blankPref, magazineСontainer);
            blank.gameObject.SetActive(true);
            allObjects.Add(blank.gameObject);
        }
    }

    private void ClearAllAmmo()
    {
        if (allObjects.Count != 0)
        {
            allObjects.ForEach(Destroy);
            allObjects.Clear();
        }
    }

    private void DischargeAmmo()
    {
        var amount = dischargeQuantitySettings.GetValue();
        if (amount < 1) return;

        var dischargedAmmo = new List<SingleAmmo>();
        for (int i = 0; i < amount; i++)
            dischargedAmmo.Add(currentMagazine.GetAmmo());

        if (dischargedAmmo.Count < 1) return;

        var caliber = dischargedAmmo.First().Caliber;
        var dictAmmo = new Dictionary<SingleAmmo, Stack<SingleAmmo>>();
        foreach (var ammo in dischargedAmmo)
            if (dictAmmo.ContainsKey(ammo))
                dictAmmo[ammo].Push(ammo);
            else
            {
                dictAmmo.Add(ammo, new Stack<SingleAmmo>());
                dictAmmo[ammo].Push(ammo);
            }

        var typesOfAmmo = dictAmmo.Keys;
        var dictBoxes = new Dictionary<SingleAmmo, List<AmmoBox>>();
        var allBoxes = GetAllAmmoBoxes();

        foreach (var ammoBox in allBoxes)
            if (ammoBox.Data.Caliber == caliber && typesOfAmmo.Contains(ammoBox.Data.AmmoType))
                if (dictBoxes.ContainsKey(ammoBox.Data.AmmoType))
                    dictBoxes[ammoBox.Data.AmmoType].Add(ammoBox);
                else
                    dictBoxes.Add(ammoBox.Data.AmmoType, new List<AmmoBox> {ammoBox});

        var orderedBoxes = dictBoxes.ToDictionary(elem => elem.Key,
            elem => elem.Value.OrderByDescending(box => box.CurrentNumberAmmo).ToList());

        foreach (var type in typesOfAmmo)
        {
            if (orderedBoxes.TryGetValue(type, out var boxes))
                boxes.ForEach(box => box.Load(dictAmmo[type]));
            while (dictAmmo[type].Count > 0)
            {
                var newAmmoBox = CreateNewAmmoBox(type);
                newAmmoBox.Load(dictAmmo[type]);
            }
        }
    }

    private void ChargeAmmo()
    {
        var amount = chargeQuantitySettings.GetValue();
        if (amount < 1) return;
        if (currentMagazine.IsFull) return;

        for (int i = 0; i < amount; i++)
        {
            if (!currentMagazine.IsFull)
                currentMagazine.Load(selectedAmmoBox.TakeBullet());
        }

        if (!selectedAmmoBox.IsEmpty)
        {
            var freeSlots = currentMagazine.MaxAmmoAmount - currentMagazine.CurrentNumberAmmo;
            chargeQuantitySettings.MaxValue = freeSlots < selectedAmmoBox.CurrentNumberAmmo
                ? freeSlots
                : selectedAmmoBox.CurrentNumberAmmo;
        }
        else
        {
            DestroyAmmoBox(selectedAmmoBox);
        }
    }

    private void OnChangeAmmoType(int value)
    {
        if (value == 0)
        {
            chargeSelectAmmoBox.options = new List<TMP_Dropdown.OptionData>();
            chargeQuantitySettings.ResetValue();
            selectedAmmoBox = null;
            chargeSelectAmmoBox.interactable = false;
            chargeQuantitySettings.SetInteractable(false);
            return;
        }

        chargeSelectAmmoBox.interactable = true;
        MagazineFullCheck();
        UpdateAmmoBoxSelection();
    }

    private void OnChangeAmmoBox(int value)
    {
        selectedAmmoBox = ammoBoxesByTypeAmmo[value];

        if ((selectedAmmoBox != null || ammoBoxesByTypeAmmo.Count > 0) && !currentMagazine.IsFull)
            chargeQuantitySettings.SetInteractable(true);

        var freeSlots = currentMagazine.MaxAmmoAmount - currentMagazine.CurrentNumberAmmo;
        chargeQuantitySettings.MaxValue = freeSlots < selectedAmmoBox.CurrentNumberAmmo
            ? freeSlots
            : selectedAmmoBox.CurrentNumberAmmo;
    }

    private AmmoBox CreateNewAmmoBox(SingleAmmo typeAmmo)
    {
        var newAmmoBox = Instantiate(AmmoTypesAndBoxes.AmmoBoxDictionary[typeAmmo]);
        newAmmoBox.GetComponent<AmmoBox>().ToEmpty();
        InventoryController.Instance.SelectedInventoryGrid = LocationInventory.Instance.LocationInventoryGrid;
        InventoryController.Instance.AddItemToInventory(newAmmoBox.GetComponent<InventoryItem>(), false);
        return newAmmoBox.GetComponent<AmmoBox>();
    }

    private List<AmmoBox> GetAllAmmoBoxes()
    {
        var allBoxes = new List<AmmoBox>();

        allBoxes.AddRange(LocationInventory.GetItemsFromInventoryByType<AmmoBox>().Reverse());
        TheRevenantsAge.GlobalMapController.ChosenGroup.CurrentGroupMembers.ToList().ForEach(character =>
            allBoxes.AddRange(character.GetItemsFromAllInventoriesByType<AmmoBox>().Reverse()));

        return allBoxes;
    }

    private List<SingleAmmo> GetAllTypesAmmo()
    {
        if (currentMagazine == null) return null;

        var allBoxes = GetAllAmmoBoxes();

        return allBoxes.Where(box => box.Data.Caliber == currentMagazine.Data.Caliber && !box.IsEmpty)
            .Select(box => box.Data.AmmoType)
            .Distinct().ToList();
    }

    private List<AmmoBox> GetAmmoBoxesByTypeAmmo(SingleAmmo ammoType)
    {
        return GetAllAmmoBoxes().Where(box =>
                box.Data.Caliber == currentMagazine.Data.Caliber && box.Data.AmmoType == ammoType && !box.IsEmpty)
            .OrderBy(box => box.CurrentNumberAmmo).ToList();
    }
    
    private void UpdateAmmoTypeSelection()
    {
        allTypesAmmo = GetAllTypesAmmo();
        var selectedType = chargeSelectTypeAmmo.options[chargeSelectTypeAmmo.value].text;
        chargeSelectTypeAmmo.options = new List<TMP_Dropdown.OptionData> {new("Выберите")};
        chargeSelectTypeAmmo.options.AddRange(allTypesAmmo
            .Select(ammo => new TMP_Dropdown.OptionData(text: ammo.Caliber.ToString())).ToList());
        var index = chargeSelectTypeAmmo.options.FindIndex(option => option.text == selectedType);
        if (index >= 0)
        {
            chargeSelectTypeAmmo.value = index;
            chargeSelectTypeAmmo.captionText.text = chargeSelectTypeAmmo.options[index].text;
        }
        else
        {
            chargeSelectTypeAmmo.value = 0;
            chargeSelectTypeAmmo.onValueChanged?.Invoke(0);
        }

    }

    private void UpdateAmmoBoxSelection()
    {
        if (!chargeSelectAmmoBox.interactable) return;

        ammoBoxesByTypeAmmo = GetAmmoBoxesByTypeAmmo(allTypesAmmo[chargeSelectTypeAmmo.value - 1]);
        chargeSelectAmmoBox.options = new List<TMP_Dropdown.OptionData>(ammoBoxesByTypeAmmo.Select(box =>
            new TMP_Dropdown.OptionData(
                $"Коробка {box.Data.Caliber} {box.CurrentNumberAmmo}/{box.Data.Capacity}")));
        if (selectedAmmoBox != null)
        {
            var indexSelectedAmmoBox = ammoBoxesByTypeAmmo.FindIndex(box => box == selectedAmmoBox);
            chargeSelectAmmoBox.value = indexSelectedAmmoBox;
        }
        else
        {
            chargeSelectAmmoBox.value = 0;
            chargeSelectAmmoBox.onValueChanged?.Invoke(0);
        }
    }

    private void DestroyAmmoBox(AmmoBox ammoBox)
    {
        var indexRemovedBox = ammoBoxesByTypeAmmo.FindIndex(box => box == ammoBox);
        ammoBoxesByTypeAmmo.RemoveAt(indexRemovedBox);
        chargeSelectAmmoBox.options.RemoveAt(indexRemovedBox);
        Destroy(ammoBox.gameObject);
        if (ammoBoxesByTypeAmmo.Count > 0)
            chargeSelectAmmoBox.onValueChanged?.Invoke(Math.Clamp(indexRemovedBox, 0, ammoBoxesByTypeAmmo.Count - 1));
        else
        {
            chargeSelectAmmoBox.captionText.text = String.Empty;
            chargeSelectAmmoBox.interactable = false;
            chargeQuantitySettings.ResetValue();
        }
    }

    private void UpdateDischargeQuantitySettings()
    {
        dischargeQuantitySettings.MaxValue = currentMagazine.CurrentNumberAmmo;
    }

    private void MagazineFullCheck()
    {
        if (currentMagazine == null) return;
        if (currentMagazine.IsFull || !chargeSelectAmmoBox.interactable)
        {
            chargeQuantitySettings.SetInteractable(false);
        }
        else
            chargeQuantitySettings.SetInteractable(true);
    }
}