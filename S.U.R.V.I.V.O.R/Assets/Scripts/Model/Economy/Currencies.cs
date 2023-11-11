using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheRevenantsAge
{
    public class Currencies
    {
        private Dictionary<MoneyType, Money> moneyDictionary;

        public IReadOnlyCollection<Money> MoneyAmount => moneyDictionary.Values;

        public event Action<Money> moneyChanged;

        public Currencies()
        {
            moneyDictionary = new Dictionary<MoneyType, Money>();
            foreach (MoneyType moneyType in Enum.GetValues(typeof(MoneyType)))
            {
                moneyDictionary.Add(moneyType, new Money(moneyType, 0));
            }
        }

        public Currencies(IEnumerable<KeyValuePair<MoneyType, Money>> moneyDictionary)
        {
            this.moneyDictionary = new Dictionary<MoneyType, Money>(moneyDictionary);
            foreach (MoneyType moneyType in Enum.GetValues(typeof(MoneyType)))
            {
                this.moneyDictionary.TryAdd(moneyType, new Money(moneyType, 0));
            }
        }

        public Currencies(IEnumerable<KeyValuePair<MoneyType, int>> moneyDictionary)
            : this
            (moneyDictionary.ToDictionary(u => u.Key,
                u => new Money(u.Key, (uint)u.Value)))
        {
        }

        public Money this[MoneyType moneyType]
        {
            get => moneyDictionary[moneyType];
            set
            {
                moneyDictionary[moneyType] = value;
                moneyChanged?.Invoke(value);
            }
        }
    }
}