using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    [CreateAssetMenu(menuName = "Events/AtomicActions/FullHealGroup", fileName = "new FullHealGroupAction", order = 60)]
    public class FullHealGroup: AtomicAction
    {
        public override void Rise()
        {
            foreach (var character in GlobalMapController.ChosenGroup.CurrentGroupMembers)
                foreach (var part in character.ManBody.BodyParts)
                    part.Hp = part.MaxHp;
        }
    }
}