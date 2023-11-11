using TheRevenantsAge;

namespace TheRevenantsAge
{
    public class SemiAutomaticRifle : Gun
    {
        public override HandlingTypes HandlingType { get => HandlingTypes.SemiAutoRifle; }

        protected override int GetAmountOfShots(CharacterSkills skills)
        {
            switch (currentFireType)
            {
                case FireType.SemiAutomatic:
                    return 1;
                case FireType.Burst:
                    return 3;//TODO Сделать константой, зависящей от навыков обращения с оружием и переопределить методы для каждого типа оружия. 
                default:
                    return 1;
            }
        }
    }
}
