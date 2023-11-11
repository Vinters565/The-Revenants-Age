using System;
using System.Globalization;
using TheRevenantsAge;
using TMPro;
using UnityEngine;

namespace Interface.Inventory
{
    public class MedicineHealPointsDrawer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI hpText;
        private Medicine medicine;

        public void Awake()
        {
            medicine = GetComponent<Medicine>();
            medicine.HealingPointsChanged += MedicineOnHealingPointsChanged;
            MedicineOnHealingPointsChanged();
        }

        private void MedicineOnHealingPointsChanged()
        {
            hpText.text = Math.Round(medicine.CurrentHealingPoints).ToString(CultureInfo.InvariantCulture);
        }
    }
}