using System;
using System.Collections.Generic;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public interface IWearClothes
    {
        public bool Wear(Clothes clothesToWear);
        public Clothes UnWear(ClothType clothType, bool isNeedToDropItems = true);
        public IEnumerable<Clothes> GetClothes();
        public event Action<float, Vector3> WhenDamagedOrRecoveryArmor;
        
    }
}