using System.Collections.Generic;
using UnityEngine;

namespace TheRevenantsAge
{
    public interface IGameEvent
    {
        public string Name { get; }
        public Sprite Image { get; }
        public string Description { get; }
        public IReadOnlyCollection<IGameEventAction> Actions { get; }
    }
}