using System;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public interface IFightEntity: IEntity
    {
        public int SpeedInFightScene { get; set; }
        public float Initiative { get; set; }
        public Aimer Aimer { get; }
        public event Action<float, Vector3> WhenDamagedOrHealed;
        public event Action<float, Vector3> WhenDamagedOrRecoveryArmor;
        public void Attack(Vector3 targetPoint, AttackType type);
    }
}