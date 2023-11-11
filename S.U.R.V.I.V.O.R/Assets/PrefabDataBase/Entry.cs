using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DataBase
{
    [System.Serializable]
    public class Entry
    {
        [SerializeField] private string guid;
        [SerializeField] private string address;
        [SerializeField] private string resourcesPath;
        public string Guid => guid;
       
        public string Address => address;
        public string ResourcesPath => resourcesPath;

        private string assetPath;
        public string AssetPath
        {
            get
            {
                if (string.IsNullOrEmpty(assetPath))
                {
                    assetPath = AssetDatabase.GUIDToAssetPath(guid);
                }
                return assetPath;
            }
        }

        private Object asset;
        public Object Asset
        {
            get
            {
                if (asset == null)
                {
                    asset = AssetDatabase.LoadAssetAtPath<Object>(AssetPath);
                }
                return asset;
            }
        }

        public Entry([NotNull] string guid, [NotNull] string address, [NotNull] string resourcesPath)
        {
            this.guid = guid ?? throw new ArgumentNullException(nameof(guid));
            this.address = address ?? throw new ArgumentNullException(nameof(address));
            this.resourcesPath = resourcesPath ?? throw new ArgumentNullException(nameof(resourcesPath));
        }
        
  
    }
}