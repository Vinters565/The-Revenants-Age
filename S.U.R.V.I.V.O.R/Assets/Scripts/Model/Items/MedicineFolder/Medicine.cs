using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace TheRevenantsAge
{
    [RequireComponent(typeof(BaseItem))]
    public class Medicine : MonoBehaviour
    {
        public enum HealthPropertyToMedicine
        {
            Broking,
            Bleeding,
            Propitaling
        }

        public static IHealthPropertyVisitor GetAdditionalPropertyByEnum(HealthPropertyToMedicine healthPropertyToMedicine)
        {
            switch (healthPropertyToMedicine)
            {
                case HealthPropertyToMedicine.Broking:
                    return new Broking();
                case HealthPropertyToMedicine.Bleeding:
                    return new Bleeding();
                case HealthPropertyToMedicine.Propitaling:
                    var extraTurns = Random.Range(-2, 2);
                    var extraHp = Random.Range(-5, 5);
                    return new Propitaling(Propitaling.PROPITALING_DEFAULT_HP_AMOUNT + extraHp,
                        Propitaling.PROPITALING_DEFAULT_TURNS_AMOUNT + extraTurns);
                default:
                    return null;
            }
        }
        
        public static IHealthPropertyVisitor GetRemovablePropertyByEnum(HealthPropertyToMedicine healthPropertyToMedicine)
        {
            switch (healthPropertyToMedicine)
            {
                case HealthPropertyToMedicine.Broking:
                    return new Broking();
                case HealthPropertyToMedicine.Bleeding:
                    return new Bleeding();
                default:
                    return null;
            }
        }

        [SerializeField] private MedicineData data;
        private float currentHealingPoints;

        public MedicineData Data => data;

        public float CurrentHealingPoints
        {
            get => currentHealingPoints;
            set
            {
                currentHealingPoints = value;
                HealingPointsChanged?.Invoke();
            }
        }

        public event Action HealingPointsChanged;

        public void Awake()
        {
            currentHealingPoints = data.MaxHealingPoints;
        }

        public float HealOnlyHp(IAlive iAlive)
        {
            float result;
            var hpToHeal = iAlive.MaxHp - iAlive.Hp;
            if (hpToHeal < currentHealingPoints)
            {
                CurrentHealingPoints -= hpToHeal;
                result = hpToHeal;
                iAlive.Hp = iAlive.MaxHp;
            }
            else
            {
                iAlive.Hp += CurrentHealingPoints;
                result = CurrentHealingPoints;
                Destroy(gameObject);
            }

            return result;
        }

        public float HealOnlyProperties(IAlive iAlive)
        {
            var result = CurrentHealingPoints;
            var removableHealthProperties = data.RemovableHealthProperties
                .Select(x => (GetRemovablePropertyByEnum(x.HealthPropertyToMedicine),x.WeightToDeleteProperty))
                .ToArray();
            foreach (var iProperty in  removableHealthProperties)
            {
                var property = iProperty.Item1;
                var weight = iProperty.WeightToDeleteProperty;
                
                if(weight > currentHealingPoints)
                    continue;
                
                if (iAlive.Health.DeleteProperty(property))
                    CurrentHealingPoints -= weight;
            }

            if (CurrentHealingPoints == 0)
            {
                Destroy(gameObject);
            }

            return result - currentHealingPoints;
        }

        public void HealFull(IAlive iAlive)
        {
            HealOnlyProperties(iAlive);
            HealOnlyHp(iAlive);
        }
    }
}

