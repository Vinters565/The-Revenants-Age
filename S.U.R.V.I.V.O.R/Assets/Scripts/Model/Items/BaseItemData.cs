using Interface.Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheRevenantsAge
{
    [CreateAssetMenu(fileName = "New ItemData", menuName = "Data/Item Data", order = 50)]
    public sealed class BaseItemData : ScriptableObject
    {
        [FormerlySerializedAs("name")]
        [SerializeField] private string itemName;
        [TextArea(5, 8)][SerializeField] private string description;
        [SerializeField] private Vector2Int size;
        [SerializeField] private float weight;
        [SerializeField] private Sprite icon;
        [FormerlySerializedAs("itemRarityType")] [SerializeField] private ItemType itemType;
    
        public string ItemName => itemName;
        public Vector2Int Size => size;
        public string Description => description;
        public float Weight => weight;
        public Sprite Icon => icon;

        public ItemType ItemType => itemType;
    }
}