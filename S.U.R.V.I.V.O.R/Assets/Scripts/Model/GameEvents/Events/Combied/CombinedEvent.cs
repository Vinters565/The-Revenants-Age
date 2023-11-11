using System.Collections.Generic;
using Extension;
using UnityEngine;

namespace TheRevenantsAge
{
    [CreateAssetMenu(menuName = "Events/CombinedEvent", fileName = "new CombinedEvent", order = 60)]
    public class CombinedEvent: ScriptableGameEvent
    {
        [SerializeField] private string eventName;
        [SerializeField] private Sprite image;
        [TextArea(10, 100)] [SerializeField] private string eventDescription;
        [NamedArray("text")] [SerializeField] private List<CombinedAction> actions;

        public override Sprite Image => image;
        public override string Name => eventName;
        public override string Description => eventDescription;
        public override IReadOnlyCollection<IGameEventAction> Actions => actions;
    }
}