using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    [CreateAssetMenu(menuName = "Events/AtomicActions/DamageGroup", fileName = "new DamageGroupAction", order = 53)]
    public class DamageGroup : AtomicAction
    {
        [SerializeField] private float damage;
        public override void Rise()
        {
            foreach (var character in GlobalMapController.ChosenGroup.CurrentGroupMembers)
            foreach (var part in character.ManBody.BodyParts)
                part.Hp -= damage;
        }
    }
}