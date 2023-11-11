using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace TheRevenantsAge
{
    public class GameEventsDatabase : ScriptableObject
    {
        private const string NAME = "GameEventsDatabase";
        
        private static GameEventsDatabase instance;

        public static GameEventsDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<GameEventsDatabase>(NAME);
                    if (instance == null)
                    {
#if UNITY_EDITOR
                        var newDatabase = ScriptableObject.CreateInstance<GameEventsDatabase>();
                        newDatabase.name = "GameEventsDatabase";
                        UnityEditor.AssetDatabase.CreateAsset(newDatabase, $"Assets/Resources/{NAME}.asset");
                        instance = newDatabase;
#else
                        throw new Exception("Нет базы данных ивентов");
#endif
                    }
                }

                return instance;
            }
        }

        [SerializeField] private SerializedDictionary<GameEventName, ScriptableGameEvent> gameEvents;
        public IGameEvent this[GameEventName gameEventName] => gameEvents[gameEventName];
    }
}