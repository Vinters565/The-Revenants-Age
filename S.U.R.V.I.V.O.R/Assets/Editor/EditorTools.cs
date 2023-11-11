using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace EditorNamespace
{
    public static class EditorTools
    {
        [MenuItem("Tools/Missing Scripts/Find In Assets")]
        static void FindMissingScriptsInAssets()
        {
            foreach (var gameObject in GetObjectsWithMissingScriptsInAssets())
            {
                Debug.Log($"Find missing script on {gameObject}", gameObject);
            }
        }

        static IEnumerable<GameObject> GetObjectsWithMissingScriptsInAssets()
        {
            foreach (var assetGuid in AssetDatabase.FindAssets($"t:{nameof(GameObject)}"))
            {
                var path = AssetDatabase.GUIDToAssetPath(assetGuid);
                if (!path.Contains(".prefab")) continue;
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (gameObject.GetComponents<Component>().Any(x => x == null))
                {
                    yield return gameObject;
                }
            }
        }

        [MenuItem("Tools/Open Save Directory")]
        static void OpenSaveDirectory()
        {
            Process.Start("explorer.exe", "/select, \"" + Application.persistentDataPath.Replace(@"/", @"\") + "\"");
        }
    }
}