using System.Collections.Generic;
using UnityEngine;

namespace TheRevenantsAge
{
    [System.Serializable]
    public class CombinedAction: IGameEventAction
    {
        [SerializeField] private string onButtonText;
        [SerializeField] private string name;
        [TextArea(5, 100)][SerializeField] private string description;
        [SerializeField] private List<AtomicAction> atomicActions;
        public string OnButtonText => onButtonText;
        public string Name => name;
        public string Description => description;

        public void Rise()
        {
            foreach (var atomicAction in atomicActions)
                atomicAction.Rise();
        }
    }
}