using System;
using System.Collections.Generic;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public class BaseEntityWithWeapon: BaseEntity, IEntityWithWeapon
    {
        protected Weapon chosenWeapon;

        public Weapon ChosenWeapon
        {
            get => chosenWeapon;
            set => chosenWeapon = value;
        }
        
        public virtual IEnumerable<Weapon> GetWeapons()
        {
            yield break;
        }
    }
}