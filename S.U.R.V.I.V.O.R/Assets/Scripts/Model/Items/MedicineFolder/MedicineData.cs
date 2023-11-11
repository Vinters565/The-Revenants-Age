using System;
using System.Collections.Generic;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    [CreateAssetMenu(fileName = "New MedicineData", menuName = "Data/Medicine Data", order = 50)]
    [Serializable]
    public class MedicineData : ScriptableObject
    {
        [SerializeField] private float maxHealingPoints;
        [SerializeField] private List<RemovablePropertyWeight> removableHealthProperties;

        public float MaxHealingPoints => maxHealingPoints;

        public List<RemovablePropertyWeight> RemovableHealthProperties => removableHealthProperties;
    }
}