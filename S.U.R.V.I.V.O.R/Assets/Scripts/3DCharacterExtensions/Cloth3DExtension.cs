using UnityEngine;

namespace TheRevenantsAge
{
    public class Cloth3DExtension: MonoBehaviour
    {
        [SerializeField] private MeshHideAsset hideAsset;
        [SerializeField] private HidePaths hiddenPath;
        
        public MeshHideAsset HideAsset => hideAsset;
        public HidePaths HiddenPath => hiddenPath;
    }
}