using System;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public interface IAlive
    {
        public float MaxHp { get;  set; }
        public float Hp { get; set; }
        public PropertyManager Health { get; }
        public event Action Died;
        public bool IsDied { get; }
    }
}