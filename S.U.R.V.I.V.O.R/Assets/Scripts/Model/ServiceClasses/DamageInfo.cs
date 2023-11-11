using UnityEngine;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public struct DamageInfo
    {
        public readonly float damageToBody;
        public readonly float damageToArmor;
        public readonly Vector3 bodyPartPosition;

        public DamageInfo(float damageToBody, float damageToArmor, Vector3 position)
        {
            this.damageToBody = damageToBody;
            this.damageToArmor = damageToArmor;
            bodyPartPosition = position;
        }
    }
}