using System.Collections.Generic;
using UnityEngine;

namespace TheRevenantsAge
{
    
    public abstract class ScriptableGameEvent: ScriptableObject, IGameEvent
    {
        public abstract string Name { get; }
        public abstract Sprite Image { get; }
        public abstract string Description { get; }
        public abstract IReadOnlyCollection<IGameEventAction> Actions { get; }
    }
}