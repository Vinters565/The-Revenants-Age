using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheRevenantsAge
{
    [Serializable]
    public class FightChance
    {
        [FormerlySerializedAs("fight")] [SerializeField] private FightInfo fightInfo;
        [SerializeField] [Min(1)] private int weightChance = 1;
    
        public FightInfo FightInfo => fightInfo;
        public int WeightChance => weightChance;
    }
}