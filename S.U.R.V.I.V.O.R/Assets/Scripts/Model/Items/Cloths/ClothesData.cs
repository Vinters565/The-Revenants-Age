using UnityEngine;

namespace TheRevenantsAge
{
    [CreateAssetMenu(fileName = "New ClothData", menuName = "Data/Cloth Data", order = 50)]
    public class ClothesData : ScriptableObject
    {
        [SerializeField] private int maxArmor;
        [SerializeField] private Vector2Int inventorySize;
        [SerializeField] private int warm;
        [SerializeField] private ClothType clothType;
    
        public int MaxArmor => maxArmor;
        public Vector2Int InventorySize => inventorySize;
        public int Warm => warm;
        public ClothType ClothType => clothType;
    }
}