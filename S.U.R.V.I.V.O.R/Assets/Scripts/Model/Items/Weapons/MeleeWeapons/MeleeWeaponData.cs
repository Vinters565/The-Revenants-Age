using UnityEngine;

namespace TheRevenantsAge
{
    [CreateAssetMenu(fileName = "New MeleeWeaponData", menuName = "Data/MeleeWeapon Data", order = 50)]
    public class MeleeWeaponData : ScriptableObject
    {
        public enum HandlingTypes
        {
            Short,
            Long
        }
        
        [SerializeField] [Min(0)] private float damage;
        [SerializeField] [Range(0, 1)] private float penetrating;
        [SerializeField] [Min(0)] private float ergonomics;
        [SerializeField] private HandlingTypes handlingType;

        public float Ergonomics => ergonomics;

        public float Damage => damage;

        public float Penetrating => penetrating;

        public HandlingTypes HandlingType => handlingType;

        public float PenetratingDamage => Damage * Penetrating;
        public float NormalDamage => Damage - PenetratingDamage;
    }
}