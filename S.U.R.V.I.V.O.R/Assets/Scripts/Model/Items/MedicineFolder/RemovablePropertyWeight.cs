using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheRevenantsAge
{
    [Serializable]
    public class RemovablePropertyWeight
    {
        [SerializeField] private float weightToDeleteProperty;
        [SerializeField] private Medicine.HealthPropertyToMedicine healthPropertyToMedicine;

        public float WeightToDeleteProperty => weightToDeleteProperty;
        public Medicine.HealthPropertyToMedicine HealthPropertyToMedicine => healthPropertyToMedicine;
    }
}