using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace TheRevenantsAge
{
    public static class Game
    {
        public const string PREFAB_2D_POSTFIX = "2D";
        public const string PREFAB_3D_POSTFIX = "3D";
        
        private static Type[] allTypes;
        public static readonly bool IsDebug = false;
        public static bool IsMainMenu { get; private set; }
        public static bool Is2D { get; private set; }
        public static bool Is3D { get; private set; }

        static Game()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChance;
            UpdateStatus(SceneManager.GetActiveScene());
            FindAllTypes();
            
#if UNITY_EDITOR
            IsDebug = true;
#endif
        }

        public static IReadOnlyCollection<Type> GetAllOurType() => allTypes;

        
        private static void FindAllTypes()
        {
            allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.ManifestModule.Name == "Assembly-CSharp.dll")
                .SelectMany(x => x.GetTypes())
                .ToArray();
        }

        private static void OnActiveSceneChance(Scene oldScene, Scene newScene)
        {
            UpdateStatus(newScene);
        }

        private static void UpdateStatus(Scene newScene)
        {
            var buildIndex = newScene.buildIndex;

            switch (buildIndex)
            {
                case (int) SceneName.GlobalMapScene:
                    IsMainMenu = false;
                    Is2D = true;
                    Is3D = false;
                    break;
                case (int) SceneName.MainMenu:
                    IsMainMenu = true;
                    Is2D = false;
                    Is3D = false;
                    break;
                default:
                    IsMainMenu = false;
                    Is2D = false;
                    Is3D = true;
                    break;
            }
        }
        
        
    }
}