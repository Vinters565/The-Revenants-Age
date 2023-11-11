using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheRevenantsAge
{
    [RequireComponent(typeof(BaseItem))]
    public class  AmmoBox : MonoBehaviour, IBaseItemComponent
    {
        [SerializeField] private AmmoBoxData data;
    
        private Stack<SingleAmmo> ammoStack;
        public SingleAmmo TakeBullet() => ammoStack.Pop();
        public int CurrentNumberAmmo => ammoStack.Count;
        public bool IsEmpty => ammoStack.Count == 0;
        public bool IsFull => ammoStack.Count == data.Capacity;
        public void ToEmpty() => ammoStack = new Stack<SingleAmmo>();

        public AmmoBoxData Data => data;

        public Stack<SingleAmmo> AmmoStack => ammoStack;

        private void Awake()
        {
            ammoStack = new Stack<SingleAmmo>(data.Capacity);
            for (var i = 0; i < data.Capacity; i++)
                ammoStack.Push(data.AmmoType);
        }
    
        public void Load(SingleAmmo ammo)
        {
            if (ammo.Caliber != Data.Caliber) return;
            if (ammoStack.Count + 1 > data.Capacity) return;
            ammoStack.Push(ammo);
        }
    
        public void Load(Stack<SingleAmmo> ammo)
        {
            if (ammo.First().Caliber != Data.Caliber) return;
            while (!IsFull && ammo.Count > 0)
                Load(ammo.Pop());
        }

        public ComponentState CreateState()
        {
            return new AmmoBoxState
            {
                ammoResourcesPaths = AmmoStack.Select(x => x.ResourcesPath).ToArray(),
            };
        }

        public void Restore(ComponentState state)
        {
            if (state is not AmmoBoxState ammoBoxState) return;
            ammoStack = new Stack<SingleAmmo>(ammoBoxState.AmmoScriptableObjects);
        }

        public void ShowTooltip(string itemName)
        {
            var descr = $"Вместимость: {data.Capacity}\nВсего патронов: {CurrentNumberAmmo}";
        }
    }
}