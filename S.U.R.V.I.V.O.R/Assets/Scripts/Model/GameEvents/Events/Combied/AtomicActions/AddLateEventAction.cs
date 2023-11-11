using UnityEngine;

namespace TheRevenantsAge
{
    [CreateAssetMenu(menuName = "Events/AtomicActions/AddLateEventAction", order = 60)]
    public class AddLateEventAction: AtomicAction
    {
        [SerializeField] private ScriptableGameEvent gameEvent;
        //если 0, то активируется мгновенно
        [SerializeField, Min(0)] private int turnsBeforeActivate;
        public override void Rise()
        {
            GameEventsController.Instance.AddLateEvent(gameEvent, (uint)turnsBeforeActivate);
        }
    }
}