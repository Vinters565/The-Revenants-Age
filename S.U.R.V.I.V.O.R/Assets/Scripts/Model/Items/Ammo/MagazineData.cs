using UnityEngine;

namespace TheRevenantsAge
{
    [CreateAssetMenu(fileName = "New MagazineData", menuName = "Data/Magazine Data", order = 50)]
    public class MagazineData : ScriptableObject
    {
        [SerializeField] private Caliber caliber;
        [SerializeField] private int maxAmmoAmount;
        [SerializeField] private SingleAmmo defaultAmmo;

        public Caliber Caliber => caliber;

        public int MaxAmmoAmount => maxAmmoAmount;
    
        public SingleAmmo DefaultAmmo => defaultAmmo;
    }
}
