using System.Linq;
using UnityEditor;

namespace DataBase
{
    public class PostProcessor : AssetPostprocessor
    {
        
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!importedAssets
                    .Union(deletedAssets)
                    .Union(movedAssets)
                    .Any(x => x.Contains(".prefab")))
                return;
            
            var prefabDataBaseController = EditorPrefabDataBaseController.CreateInstance();
            var processed = false;
            movedAssets = movedAssets.Except(importedAssets).Except(deletedAssets).ToArray();
            foreach (var importedAsset in importedAssets)
            {
                if (importedAsset.Contains(".prefab"))
                {
                    processed = true;
                    prefabDataBaseController.ProcessImportedPrefab(importedAsset);
                }
            }

            foreach (var deletedAsset in deletedAssets)
            {
                if (deletedAsset.Contains(".prefab"))
                {
                    processed = true;
                    prefabDataBaseController.ProcessDeletedPrefab(deletedAsset);
                }
            }

            foreach (var movedAsset in movedAssets)
            {
                if (movedAsset.Contains(".prefab"))
                {
                    processed = true;
                    prefabDataBaseController.ProcessMovedPrefab(movedAsset);
                }
            }

            if (processed)
                EditorApplication.delayCall += AssetDatabase.Refresh;
            prefabDataBaseController.ReleaseInstance();
        }
    }
}