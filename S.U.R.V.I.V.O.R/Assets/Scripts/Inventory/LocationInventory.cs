using System.Collections.Generic;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Inventory
{
    public class LocationInventory : MonoBehaviour
    {
        public static LocationInventory Instance { get; private set; }

        [SerializeField] private Text text;

        [FormerlySerializedAs("itemGrid")] [SerializeField]
        private InventoryGrid inventoryGrid;

        public InventoryGrid LocationInventoryGrid => inventoryGrid;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
                Destroy(gameObject);
        }

        private void Start()
        {
            OnLocationChanged(TheRevenantsAge.GlobalMapController.ChosenGroup.Location);
        }

        private void OnEnable()
        {
            TheRevenantsAge.GlobalMapController.ChosenGroup.GroupMovementLogic.LocationChanged += OnLocationChanged;
            TheRevenantsAge.GlobalMapController.ChosenGroupChange += OnChosenGroupChange;
        }

        private void OnDisable()
        {
            TheRevenantsAge.GlobalMapController.ChosenGroup.GroupMovementLogic.LocationChanged -= OnLocationChanged;
            TheRevenantsAge.GlobalMapController.ChosenGroupChange -= OnChosenGroupChange;
        }

        private void OnLocationChanged(Location loc)
        {
            if (text != null)
                text.text = loc.name;
        }

        private void OnChosenGroupChange(Group currentGroup, Group newGroup)
        {
            currentGroup.GroupMovementLogic.LocationChanged -= OnLocationChanged;
            newGroup.GroupMovementLogic.LocationChanged += OnLocationChanged;
        }

        public static IEnumerable<T> GetItemsFromInventoryByType<T>() where T : MonoBehaviour =>
            Instance.LocationInventoryGrid.GetItemsFromInventoryByType<T>();

        public static void InsertItem(InventoryItem item) => Instance.inventoryGrid.InsertItem(item);
    }
}