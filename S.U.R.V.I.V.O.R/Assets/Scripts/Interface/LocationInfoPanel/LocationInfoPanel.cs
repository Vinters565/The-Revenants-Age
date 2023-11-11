using System;
using System.Collections.Generic;
using System.Linq;
using TheRevenantsAge;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LocationInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI locationName;

    [SerializeField] private LootAmountString gunString;
    [SerializeField] private LootAmountString ammoString;
    [SerializeField] private LootAmountString medicineString;
    [SerializeField] private LootAmountString clothesString;
    [SerializeField] private LootAmountString foodString;
    [SerializeField] private LootAmountString materialsString;

    [SerializeField] private TMP_Text descriptionText;
    private List<Color> colors;
    
    
    void Start()
    {
        colors = new List<Color>()
        {
            new (201, 91, 65),
            new (211, 120, 20),
            new (207, 189, 17),
            new (161, 188, 55),
            new (108, 183, 56),
        };
        OnLocationChanged(GlobalMapController.ChosenGroup.Location);
    }

    private void OnChosenGroupChange(Group oldGroup, Group newGroup)
    {
        oldGroup.GroupMovementLogic.LocationChanged -= OnLocationChanged;
        newGroup.GroupMovementLogic.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(Location loc)
    {
        locationName.text = loc.Data.LocationName;
        var hardNode = loc.GetComponent<HardNode>();
        if(hardNode == null)
            ReDrawSimpleLocation(loc);
        else
            ReDrawHardLocation(hardNode);
    }

    private void ReDrawSimpleLocation(Location loc)
    {
        SetAllStringsActive(true);
        descriptionText.gameObject.SetActive(false);
        var chancesDict = new Dictionary<Type, float>();
        foreach (var pair in loc.Data.AllItemsChances)
        {
            if (pair.Item == null) continue;
            chancesDict = SumDictionaries(CalculateGameObjectChances(pair.Item.gameObject, pair.WeightChance), chancesDict);  
        }
        var resultDict = new Dictionary<Type, float>
        {
            {typeof(Weapon),0},
            {typeof(Clothes),0},
            {typeof(Scrap),0},
            {typeof(Medicine),0},
            {typeof(AmmoBox),0},
            {typeof(EatableFood),0}
        };
        foreach (var key in chancesDict.Keys.ToArray())
        {
            foreach (var resultKey in resultDict.Keys.ToArray())
            {
                if (key == resultKey || resultKey.IsAssignableFrom(key))
                {
                    resultDict[resultKey] += chancesDict[key]/loc.Data.LengthOfMainArray;
                }
            }
        }
        gunString.Redraw(resultDict[typeof(Weapon)] ,colors);
        ammoString.Redraw(resultDict[typeof(AmmoBox)],colors);
        medicineString.Redraw(resultDict[typeof(Medicine)],colors);
        clothesString.Redraw(resultDict[typeof(Clothes)],colors);
        materialsString.Redraw(resultDict[typeof(Scrap)],colors);
        foodString.Redraw(resultDict[typeof(EatableFood)],colors);  
    }

    private void ReDrawHardLocation(HardNode node)
    {
        SetAllStringsActive(false);
        descriptionText.gameObject.SetActive(true);
        descriptionText.text = node.Description;
    }
    
    private void SetAllStringsActive(bool value)
    {
        gunString.GameObject().SetActive(value);
        ammoString.GameObject().SetActive(value);
        medicineString.GameObject().SetActive(value);
        clothesString.GameObject().SetActive(value);
        materialsString.GameObject().SetActive(value);
        foodString.GameObject().SetActive(value);
    }
    
    private Dictionary<Type, float> CalculateGameObjectChances(GameObject obj, float chance)
    {
        var resultDic = new Dictionary<Type, float>();
        foreach (var component in obj.GetComponents<Component>())
        {
            var type = component.GetType();
            if (type == typeof(PackedContainer))
            {
                foreach (var packedObjectComponent in (component as PackedContainer)?.ShowUnpackedItemsTypes()!)
                {
                    resultDic = SumDictionaries(resultDic, CalculateGameObjectChances(packedObjectComponent.gameObject,chance));
                }
            }
            else
            {
                if (!resultDic.ContainsKey(type)) resultDic[type] = 0;
                resultDic[type] = chance;  
            }
        }
        return resultDic;
    }
    
    
    private Dictionary<T, float> SumDictionaries<T>(Dictionary<T, float> dic1, Dictionary<T, float> dic2)
    {
        var dic3 = new Dictionary<T, float>();
        
        foreach (var key in dic1.Keys.ToArray())
        {
            if (!dic3.ContainsKey(key)) dic3[key] = 0f;
            dic3[key] += dic1[key];
        }
        
        foreach (var key in dic2.Keys.ToArray())
        {
            if (!dic3.ContainsKey(key)) dic3[key] = 0f;
            dic3[key] += dic2[key];
        }
        
        return dic3;
    }
    
    private void OnEnable()
    {
        GlobalMapController.ChosenGroupChange += OnChosenGroupChange;
        GlobalMapController.ChosenGroup.GroupMovementLogic.LocationChanged += OnLocationChanged;
    }

    private void OnDisable()
    {
        GlobalMapController.ChosenGroupChange -= OnChosenGroupChange;
        GlobalMapController.ChosenGroup.GroupMovementLogic.LocationChanged -= OnLocationChanged;
    }
}
