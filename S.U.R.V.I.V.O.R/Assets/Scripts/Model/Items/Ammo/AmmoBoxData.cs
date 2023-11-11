using UnityEngine;

namespace TheRevenantsAge
{
    [CreateAssetMenu(fileName = "New AmmoBoxData", menuName = "Data/AmmoBox Data", order = 50)]
    public class AmmoBoxData : ScriptableObject
    {
        [SerializeField] private SingleAmmo ammoType;
    
        [SerializeField] private int capacity;

        public SingleAmmo AmmoType => ammoType;

        public int Capacity => capacity;

        public Caliber Caliber => ammoType.Caliber;
    
    
    }
}