using System.Collections.Generic;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    [CreateAssetMenu(fileName = "New FightData", menuName = "Data/Fight Data", order = 50)]
    public class FightInfo : ScriptableObject
    {
        [SerializeField] private List<BaseFightEntity> enemies;
        [SerializeField] private SceneName sceneName;

        public IEnumerable<BaseFightEntity> Enemies => enemies;
        public SceneName SceneName => sceneName;
    }
}