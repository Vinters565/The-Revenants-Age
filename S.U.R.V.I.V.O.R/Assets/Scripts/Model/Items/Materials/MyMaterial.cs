using UnityEngine;

namespace TheRevenantsAge
{
    public class MyMaterial : MonoBehaviour
    {
        [SerializeField] private MaterialType materialType;

        public MaterialType MaterialType => materialType;
    }
}