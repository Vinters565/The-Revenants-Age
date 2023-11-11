using System;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public interface IFightCharacter: IFightEntity, ICharacter
    {
        public int ActionPoints { get; }
        public int TurnPoints { get; }
        public event Action RestoreEnd;
    }
}