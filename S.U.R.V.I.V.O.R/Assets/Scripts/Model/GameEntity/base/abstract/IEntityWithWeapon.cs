using System.Collections.Generic;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public interface IEntityWithWeapon: IEntity
    {
        public Weapon ChosenWeapon { get; }
        public IEnumerable<Weapon> GetWeapons();
    }
}