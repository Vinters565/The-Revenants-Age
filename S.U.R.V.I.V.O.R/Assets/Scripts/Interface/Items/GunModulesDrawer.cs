using System.Collections.Generic;
using Interface.Items;
using Inventory;
using Inventory.SpecialCells;
using UnityEngine;

[RequireComponent(typeof(Gun))]
public class GunModulesDrawer : MonoBehaviour
{
    private const float PADDING = 0;
    
    private Gun currentGun;
    private Dictionary<GunModuleType, GunModuleTracking> instantiatedImagesDict;

    [SerializeField] private float gunModulesScaleModifier;
    [SerializeField] private RectTransform scopeRectTransform;
    [SerializeField] private RectTransform suppressorRectTransform;
    [SerializeField] private RectTransform magazineRectTransform;

    private float absoluteScopeScale;
    private float absoluteSupressorScale;
    
    
    public void Awake()
    {
        currentGun = GetComponent<Gun>();
        instantiatedImagesDict = new Dictionary<GunModuleType, GunModuleTracking>
        {
            { GunModuleType.Scope, null },
            { GunModuleType.Magazine, null },
            { GunModuleType.Suppressor, null }
        };
    }

    private void OnModulesChanged(GunModuleType gunModuleType)
    {
        var gunModule = currentGun.GunModules[gunModuleType];
        if (gunModule == null)
        {
            var trackingRect = instantiatedImagesDict[gunModuleType].GetComponent<RectTransform>();
            instantiatedImagesDict[gunModuleType].gameObject.SetActive(false);
            ReDrawWithoutModule(gunModuleType);
            trackingRect.localScale *= 1 / gunModulesScaleModifier;
            instantiatedImagesDict[gunModuleType] = null;
            return;
        }
        var gunModuleTracking = gunModule.gameObject.GetComponent<GunModuleTrackerReference>().GunModuleTracking;
        if(gunModuleTracking is null) return;

        instantiatedImagesDict[gunModuleType] = gunModuleTracking;
        gunModuleTracking.gameObject.SetActive(true);
        ReDrawModule(gunModuleType, gunModuleTracking);
    }

    private void ReDrawAllModules()
    {
        OnModulesChanged(GunModuleType.Scope);
        OnModulesChanged(GunModuleType.Suppressor);
        OnModulesChanged(GunModuleType.Magazine);
    }

    private void ReDrawModule(GunModuleType gunModuleType, GunModuleTracking moduleTracking)
    {
        var rectTransformToPasteIn = GetTransformByType(gunModuleType);
        var transform = moduleTracking.transform;
        var gameObject = moduleTracking.gameObject;

        transform.SetParent(rectTransformToPasteIn);
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(true);
        transform.localScale *= gunModulesScaleModifier;

        ReSizeImage(gunModuleType,moduleTracking);
    }

    private void ReSizeImage(GunModuleType gunModuleType, GunModuleTracking gunModuleTracking)
    {
        var item = GetComponent<InventoryItem>();
        var imageTransform = item.Image.GetComponent<RectTransform>();
        var moduleRect = gunModuleTracking.GetComponent<RectTransform>();
        if (gunModuleType == GunModuleType.Scope )
        {
            var heightPart = 1 - moduleRect.pivot.y;
            var localHeightOfModule = moduleRect.rect.height * heightPart * gunModulesScaleModifier;
            absoluteScopeScale = (localHeightOfModule + PADDING) / imageTransform.rect.height;
            
            var localScale = imageTransform.localScale;
            localScale = new Vector3(localScale.x - absoluteScopeScale , localScale.y - absoluteScopeScale);
            imageTransform.localScale = localScale;
        }
        else if (gunModuleType == GunModuleType.Suppressor)
        {
            var widthPart = 1 - moduleRect.pivot.x;
            var localWidthOfModule = moduleRect.rect.width * widthPart * gunModulesScaleModifier;
            absoluteSupressorScale = (localWidthOfModule + PADDING) / imageTransform.rect.width;
            
            var localScale = imageTransform.localScale;
            localScale = new Vector3(localScale.x - absoluteSupressorScale , localScale.y - absoluteSupressorScale);
            imageTransform.localScale = localScale;
        }
    }

    private void ReDrawWithoutModule(GunModuleType gunModuleType)
    {
        var scaleToPlus = 0f;
        
        if (gunModuleType == GunModuleType.Scope)
        {
            scaleToPlus = absoluteScopeScale;
            absoluteScopeScale = 0;
        }
        else if (gunModuleType == GunModuleType.Suppressor)
        {
            scaleToPlus = absoluteSupressorScale;
            absoluteSupressorScale = 0;
        }
        
        var item = GetComponent<InventoryItem>();
        var imageTransform = item.Image.GetComponent<RectTransform>();

        var localScale = imageTransform.localScale;
        localScale = new Vector3(localScale.x + scaleToPlus , localScale.y + scaleToPlus);
        imageTransform.localScale = localScale;
    }
    

    private RectTransform GetTransformByType(GunModuleType gunModuleType)
    {
        switch (gunModuleType)
        {
            case GunModuleType.Magazine:
                return magazineRectTransform;
            case GunModuleType.Suppressor:
                return suppressorRectTransform;
            case GunModuleType.Scope:
                return scopeRectTransform;
            default: return GetComponent<RectTransform>();
        }
    }

    private void OnEnable()
    {
        if(currentGun is null) return;
        currentGun.ModulesChanged += OnModulesChanged;
        //ReDrawAllModules();
    }

    private void OnDisable()
    {
        if(currentGun is null) return;
        currentGun.ModulesChanged -= OnModulesChanged;
    }
}

