using System.Collections.Generic;
using System.Linq;
using Extension;
using Inventory;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace TheRevenantsAge
{
    [CreateAssetMenu(fileName = "New LocationData", menuName = "Data/Location Data", order = 50)]
    public class LocationData : ScriptableObject
    {
        [Header("Отображение")] [SerializeField]
        private GameObject prefab;

        [SerializeField] private string locationName;

        [Header("Основные данные")]
        [SerializeField] [Range(0,1)] private float lootChance;
    
        [NamedArray("title")]
        [FormerlySerializedAs("chancesList")]
        [SerializeField] private List<ItemChance> lootChancesList;

        [Space(10)] 
        [SerializeField] [Range(0, 1)] private float fightChance;
        [SerializeField] private List<FightChance> fightChancesList;

        [Space(10)] 
        [SerializeField] [Range(0, 1)] private float eventChance;
    
        [NamedArray("title")]
        [SerializeField] private List<GameEventChance> eventChancesList;

        private static readonly Random rnd = new();
    
        private InventoryItem[] items;
        private FightInfo[] fights;
        private IGameEvent[] events;

        public int LengthOfMainArray { get; private set; }
        public IEnumerable<ItemChance> AllItemsChances => lootChancesList;
        public string LocationName => locationName;
        public GameObject Prefab => prefab;

        private void OnEnable()
        {
            items = new InventoryItem[lootChancesList.Sum(i => i.WeightChance)];
            LengthOfMainArray = items.Length;
            var index = 0;
            foreach (var chance in lootChancesList)
                for (var i = 0; i < chance.WeightChance; i++)
                {
                    items[index] = chance.Item;
                    index++;
                }


            fights = new FightInfo[fightChancesList.Sum(i => i.WeightChance)];
            index = 0;
            foreach (var chance in fightChancesList)
                for (var i = 0; i < chance.WeightChance; i++)
                {
                    fights[index] = chance.FightInfo;
                    index++;
                }

            events = new IGameEvent[eventChancesList.Sum(i => i.WeightChance)];
            index = 0;
            foreach (var chance in eventChancesList)
                for (var i = 0; i < chance.WeightChance; i++)
                {
                    events[index] = chance.GameEvent;
                    index++;
                }
        }

        public InventoryItem GetLoot()
        {
            if (items.Length == 0)
                return null;
            if (rnd.NextDouble() < lootChance)
                return items[rnd.Next(items.Length - 1)];
            return null;
        }

        public IGameEvent GetEvent()
        {
            if (events.Length == 0)
                return null;
            if (rnd.NextDouble() < eventChance)
                return events[rnd.Next(events.Length - 1)];
            return null;
        }

        public bool CheckFight()
        {
            if (rnd.NextDouble() < fightChance)
            {
                var fightInfo = fights[rnd.Next(fights.Length)];
                var fight = new Fight(fightInfo);
                fight.Initialization();
                return true;
            }

            return false;
        }


        private void OnValidate()
        {
            foreach (var chance in lootChancesList)
                chance.Validate();
            foreach (var chance in eventChancesList)
                chance.Validate();
        }
    }
}