using UnityEditor;
using UnityEngine;

namespace DataBase
{
    public static class EditorTools
    {
        [MenuItem("Tools/Data Base/Refresh Prefab Data Base")]
        public static void RefreshPrefabDataBase()
        {
            var instance = EditorPrefabDataBaseController.CreateInstance();
            instance.Refresh();
            instance.ReleaseInstance();
            EditorApplication.delayCall += AssetDatabase.Refresh;
        }
    }
}