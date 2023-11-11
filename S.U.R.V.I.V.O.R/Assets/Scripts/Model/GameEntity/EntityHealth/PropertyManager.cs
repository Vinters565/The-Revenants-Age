using System;
using System.Collections.Generic;
using System.Linq;

namespace TheRevenantsAge
{
    public sealed class PropertyManager
    {
        // КЛАСС ЗАКРЫТ ДЛЯ ИЗМЕНЕНИЙ, необходима консультация у технического директора
        private readonly IAlive target;

        private readonly List<IHealthPropertyVisitor> healthProperties;
        public event Action HealthPropertiesChanged;

        public PropertyManager(IAlive target)
        {
            this.target = target;
            healthProperties = new List<IHealthPropertyVisitor>();
        }

        public PropertyManager(IAlive target, IEnumerable<IHealthPropertyVisitor> healthProperties)
        {
            this.target = target;
            this.healthProperties = healthProperties.ToList();
            foreach (var property in this.healthProperties)
                property.InitialActionSwitch(this.target);
        }

        public IReadOnlyCollection<IHealthPropertyVisitor> HealthProperties => healthProperties;

        public void AddProperty(IHealthPropertyVisitor property)
        {
            if (property == null)
                throw new NullReferenceException("HealthProperty is null!");
            var dubler = healthProperties.FirstOrDefault(x => x.GetType() == property.GetType());
            if (dubler != null)
            {
                DeleteProperty(dubler);
                property = property.Add(dubler);
            }

            healthProperties.Add(property);
            property.InitialActionSwitch(target);
            HealthPropertiesChanged?.Invoke();
        }

        public bool DeleteProperty(IHealthPropertyVisitor property)
        {
            var prop = healthProperties.FirstOrDefault(x => x.GetType() == property.GetType());
            if (prop == null)
                return false;
            healthProperties.Remove(prop);
            prop.FinalActionSwitch(target);
            HealthPropertiesChanged?.Invoke();
            return true;
        }

        public void OnTurnEnd()
        {
            if (healthProperties == null || healthProperties.Count == 0) return;

            var healthPropertiesAmountOnStart = HealthProperties.Count;
            var healtProp = HealthProperties.ToArray();
            for (int i = 0; i < healthPropertiesAmountOnStart; i++)
            {
                var healthProperty = healtProp[i];
                healthProperty.TurnEndActionSwitch(target);
                if (HealthProperties.Count != healthPropertiesAmountOnStart)
                {
                    i--;
                    healthPropertiesAmountOnStart -= 1;
                }
            }
        }
    }
}