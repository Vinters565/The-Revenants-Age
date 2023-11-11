using System;
using System.Collections.Generic;
using System.Linq;
using DataBase;
using UnityEngine;
using UnityEngine.Events;

namespace TheRevenantsAge
{
    [RequireComponent(typeof(Pointer), typeof(ItemCost))]
    public class BaseItem : MonoBehaviour
    {
        private ICharacter itemOwner;
        private ItemState oldState;

        private string address2D;
        private string address3D;

        [SerializeField] private BaseItemData data;
        public BaseItemData Data => data;
        public ICharacter ItemOwner
        {
            get => itemOwner;
            set
            {
                itemOwner = value;
                ItemOwnerChanged.Invoke(value);
            }
        }

        [HideInInspector] public UnityEvent<ICharacter> ItemOwnerChanged;

        private void Awake()
        {
            InitializeResourceReferences();
        }

        private void InitializeResourceReferences()
        {
            var address = GetComponent<Pointer>().Address;
            var dualGenerator = new DualAddressGenerator();
            if (string.IsNullOrEmpty(address))
                throw new Exception($"{gameObject.name} не адрессован!");
            var result = dualGenerator.GenerateDualAddress(address);
            address2D = result.Item1;
            address3D = result.Item2;
        }

        public ItemState CreateState()
        {
            var itemState = new ItemState()
            {
                address2D = address2D,
                address3D = address3D
            };

            var allComponents = GetComponents<Component>()
                .OfType<IBaseItemComponent>()
                .ToArray();
            var componentStates = new List<ComponentState>(allComponents.Length);
            foreach (var component in allComponents)
            {
                var componentState = component.CreateState();
                componentStates.Add(componentState);
                componentState.itemState = itemState;
            }


            if (Game.Is3D)
            {
                var newStatesComponentsTypes = componentStates
                    .Select(x => x.GetType())
                    .ToHashSet();
                foreach (var oldComponentState in oldState.componentStates)
                {
                    var oldComponentStateType = oldComponentState.GetType();
                    if (!newStatesComponentsTypes.Contains(oldComponentStateType))
                        componentStates.Add(oldComponentState);
                    else
                    {
                        var newState = componentStates
                            .First(x => x.GetType() == oldComponentStateType);
                        newState.CopyFromOldState(oldComponentState);
                    }
                }
            }


            itemState.componentStates = componentStates.ToArray();
            return itemState;
        }

        public void Restore(ItemState state)
        {
            if (Game.Is3D) oldState = state;

            var allComponents = GetComponents<Component>()
                .OfType<IBaseItemComponent>()
                .ToArray();

            foreach (var component in allComponents)
            foreach (var componentState in state.componentStates)
                component.Restore(componentState);
        }

        public override bool Equals(object other)
        {
            if (other is BaseItem otherBaseItem)
                return Data.ItemName == otherBaseItem.Data.ItemName;
            return false;
        }

        public override int GetHashCode()
        {
            return Data.ItemName.GetHashCode();
        }
    }
}