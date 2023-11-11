using System;
using UnityEngine;

namespace TheRevenantsAge
{
    [Serializable]
    public class GameEventChance
    {
        [SerializeField] [HideInInspector] private string title;
        
        [SerializeField] private ScriptableGameEvent gameEvent;
        [SerializeField] [Min(1)] private int weightChance;

        public IGameEvent GameEvent => gameEvent;
        public int WeightChance => weightChance;

        public void Validate()
        {
            if (gameEvent is null)
            {
                title = "No Event";
            }
            else if (gameEvent == null)
            {
                title = "Missing Event";
            }
            else
            {
                title = $"{gameEvent.Name} x{weightChance}";
            }
        }
    }
}