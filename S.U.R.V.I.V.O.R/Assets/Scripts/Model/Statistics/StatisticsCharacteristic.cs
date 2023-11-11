using UnityEngine;

namespace TheRevenantsAge
{
    public class StatisticCharacteristic
    {
        private string amount;
        private Sprite icon;
        private string characteristicName;

        public string Amount => amount;

        public Sprite Icon => icon;

        public string CharacteristicName => characteristicName;

        public StatisticCharacteristic(string amount,string characteristicName, Sprite icon)
        {
            this.amount = amount;
            this.icon = icon;
            this.characteristicName = characteristicName;
        }
    }
}