namespace TheRevenantsAge
{
    public abstract class Level
    {
        public int NeededExperienceToLevelUp { get; }

        public Level(int neededExperienceToLevelUp)
        {
            NeededExperienceToLevelUp = neededExperienceToLevelUp;
        }
    }
}