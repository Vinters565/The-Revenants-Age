using TheRevenantsAge;

namespace TheRevenantsAge
{
    public class PistoletPulemet : Gun
    {
        public override HandlingTypes HandlingType { get => HandlingTypes.AutoPistol; }

        protected override int GetAmountOfShots(CharacterSkills skills)
        {
            switch (currentFireType)
            {
                case FireType.Semi://TODO передергивание затвора при семи режиме (ТОЛЬКО В МЕТОДЕ ДЛЯ БОЛТОВОК) // При макс навыке болтовок, передергивать не нужно
                    return 1;
                case FireType.SemiAutomatic:
                    return 1;
                case FireType.Burst:
                    return 3;//TODO Сделать константой, зависящей от навыков обращения с оружием и переопределить методы для каждого типа оружия. 
                case FireType.Auto:
                    return data.FireRate;
                default:
                    return 1;
            }
        }
    }
}