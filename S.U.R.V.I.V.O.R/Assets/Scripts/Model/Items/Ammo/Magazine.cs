using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheRevenantsAge
{
    [RequireComponent(typeof(BaseItem))]
    public class Magazine : MonoBehaviour, IBaseItemComponent
    {
        [SerializeField] private MagazineData data;
        private Stack<SingleAmmo> ammoStack;

        public SingleAmmo GetAmmo()
        {
            if (ammoStack.Count == 0)
                return null;
            return ammoStack.Pop();
        }

        public void Load(AmmoBox box)
        {
            if (box.Data.Caliber != Data.Caliber) return;
            while (ammoStack.Count <= data.MaxAmmoAmount && box.CurrentNumberAmmo > 0)
                ammoStack.Push(box.TakeBullet());
        }
    
        public void Load(SingleAmmo ammo)
        {
            if (ammo.Caliber != Data.Caliber) return;
            if (ammoStack.Count + 1 > data.MaxAmmoAmount) return;
            ammoStack.Push(ammo);
        }

        public int CurrentNumberAmmo => ammoStack.Count;
        public int MaxAmmoAmount => data.MaxAmmoAmount;
        public bool IsEmpty => ammoStack.Count == 0;
        public bool IsFull => ammoStack.Count == data.MaxAmmoAmount;
        public Stack<SingleAmmo> GetAmmoStack => ammoStack;

        public MagazineData Data => data;

        private void Awake()
        {
            ammoStack = new Stack<SingleAmmo>();
            for (var i = CurrentNumberAmmo; i < data.MaxAmmoAmount; i++)
            {
                ammoStack.Push(data.DefaultAmmo);
            }
        }

        public ComponentState CreateState()
        {
            return new MagazineState
            {
                ammoResourcesPaths = ammoStack.Select(x => x.ResourcesPath).ToArray()
            };
        }

        public void Restore(ComponentState state)
        {
            if (state is not MagazineState magazineState) return;
            ammoStack = new Stack<SingleAmmo>(magazineState.AmmoScriptableObjects);
        }

        public void ShowTooltip(string itemName)
        {
            var descr = $"Вместимость: {data.MaxAmmoAmount}\nВсего патронов: {CurrentNumberAmmo}";
        }
    }
}