using System;
using Extension;
using UnityEngine;

namespace TheRevenantsAge
{
    public class SavedScriptableObject : ScriptableObject
    {
        [field: SerializeField]
        [field: ReadOnlyInspector]
        public string ResourcesPath { get; private set; }

        [field: SerializeField]
        [field: ReadOnlyInspector]
        public string Path { get; private set; }

#if UNITY_EDITOR
        private void FindPath()
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(path))
                return;
            Path = path;
            if (Path.Contains("Assets/Resources"))
                ResourcesPath = Path
                    .Replace("Assets/Resources/", String.Empty)
                    .Replace(".asset", String.Empty);
            else
                ResourcesPath = null;
        }

        private void OnValidate()
        {
            if (!UnityEditor.EditorApplication.isCompiling)
            {
                FindPath();
            }
        }
#endif
    }
}