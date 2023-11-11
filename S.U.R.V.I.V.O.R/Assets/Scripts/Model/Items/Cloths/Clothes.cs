using System;
using System.Collections.Generic;
using System.Linq;
using Extension;
using Inventory;
using UnityEngine;

namespace TheRevenantsAge
{
    [RequireComponent(typeof(BaseItem))]
    [Has3dPrefab(new []{typeof(Wearable), typeof(TheRevenantsAge.Scrap), typeof(InventoryItem)},new []{typeof(Cloth3DExtension)})]
    public class Clothes : MonoBehaviour, IBaseItemComponent
    {
        public const float TOLERANCE = 0.01f;
        [SerializeField] private ClothesData data;
        private InventoryState inventory;
        private BaseItem baseItem;
        private float currentArmor;
        public event Action<float, float> ArmorChanged; 

        public float CurrentArmor
        {
            get => currentArmor;
            set
            {
                var before = currentArmor;
                if (value < TOLERANCE)
                    currentArmor = 0;
                else
                    currentArmor = Math.Min(value, Data.MaxArmor);
                ArmorChanged?.Invoke(before, currentArmor);
            }
        }

        public InventoryState Inventory => inventory;
        public ClothesData Data => data;
        public float TotalWeight => baseItem.Data.Weight + inventory.GetItems().Sum(item => item.Data.Weight);

        private void Awake()
        {
            CurrentArmor = data.MaxArmor;
            if (inventory == null)
                inventory = new InventoryState(data.InventorySize);
            baseItem = gameObject.GetComponent<BaseItem>();
        }
    
        public ComponentState CreateState()
        {
            return new ClothesState
            {
                currentArmor = CurrentArmor,
                inventory = inventory?.GetItems().Select(x => x.GetComponent<BaseItem>().CreateState()).ToArray()
            };
        }
    
        public void Restore(ComponentState state)
        {
            if (state is not ClothesState clothesState) return;
            currentArmor = clothesState.currentArmor;
        
            
            if (clothesState.inventory != null)
            {
                if (Game.Is2D)
                {
                    inventory = new InventoryState(data.InventorySize);
                    foreach (var itemSave in clothesState.inventory)
                    {
                        var item = itemSave.InstantiateGameObj2D();
                        if (item is null) continue;
                        var invItem = item.GetComponent<InventoryItem>();
                        inventory.PlaceItem(invItem, invItem.OnGridPositionX, invItem.OnGridPositionY);
                        item.gameObject.SetActive(false);
                    }
                }

                if (Game.Is3D)
                {
                    if (data.ClothType != ClothType.Backpack)
                    {
                        var magazines = new List<ItemState>();
                        foreach (var itemSave in clothesState.inventory)
                        {
                            var magazineState = itemSave.GetComponentState<MagazineState>();
                            if(magazineState != null)
                                magazines.Add(itemSave);
                        }
                        
                        if(magazines.Count != 0)
                            BaseFightCharacter.MagazinesStatesToCharacters[baseItem.ItemOwner] = magazines;
                    }
                }
            }
        }

        
        public void OnEnable()
        {
            baseItem.ItemOwnerChanged.AddListener(OnItemOwnerChanged);
        }
        
        public void OnDisable()
        {
            baseItem.ItemOwnerChanged.RemoveListener(OnItemOwnerChanged);
        }


        private void OnItemOwnerChanged(ICharacter newOwner)
        {
            if (inventory != null)
            {
                foreach (var item in inventory.GetItems())
                {
                    item.BaseItem.ItemOwner = newOwner;
                }
            }    
        }
        
        
        private void OnDestroy()
        {
            // if (!Game.Is2D)return;
            // if (inventory != null)
            // {
            //     if (baseItem.ItemOwner != null)
            //     {
            //         if (baseItem.ItemOwner.ManBody.GetClothByType(Data.ClothType) == this)
            //         {
            //             baseItem.ItemOwner.ManBody.UnWear(Data.ClothType);
            //         }
            //     }
            //     else
            //     {
            //         foreach (var item in inventory.GetItems())
            //         {
            //             InventoryController.Instance.ThrowItemAtLocation(item);//TODO Вывести игроку на экран, что вещи выпали
            //         }
            //     }
            //     inventory.Clear();
            // }
        }
    }
}