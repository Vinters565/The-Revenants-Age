using System.Collections.Generic;
using TheRevenantsAge;

namespace TheRevenantsAge
{
    public class SemiAutoRifleHandling : GunHandlingSkill
    {
        protected override Dictionary<int, GunHandlingLevel> LevelsDictionary { get; }= new()
        {
            {1, new GunHandlingLevel(100,1, 0, 0, 1,false,0)},
            {2, new GunHandlingLevel(100,0.95f, 3, 2, 1,false,0.05f)},
            {3, new GunHandlingLevel(100,0.85f, 4, 4, 1,false,0.1f)},
            {4, new GunHandlingLevel(125,0.8f, 4, 6, 2,false,0.1f)},
            {5, new GunHandlingLevel(125,0.75f, 6, 8, 2,false,0.15f)},
            {6, new GunHandlingLevel(150,0.7f, 8, 10, 2,false,0.15f)},
            {7, new GunHandlingLevel(200,0.65f, 10, 12, 2,false,0.15f)},
            {8, new GunHandlingLevel(250,0.6f, 12, 12, 2,false,0.2f)},
            {9, new GunHandlingLevel(275,0.55f, 15, 12, 3,false,0.2f)},
            {10, new GunHandlingLevel(300,0.5f,20, 15, 3,true,0.25f)}
        };

        public SemiAutoRifleHandling(int currentLevel = 1) : base(currentLevel)
        {
        }
    }
}