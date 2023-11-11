using System;
using System.Collections.Generic;
using System.Linq;
using Extension;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public class Body : MonoBehaviour, IAlive
    {
        [NamedArray("title")] [SerializeField] private List<BodyPartRegister> bodyPartRegisters = new();
        [SerializeField] [Min(1)] private int maxCriticalLoses = 3;
        [SerializeField] [HideInInspector] private List<BodyPart> bodyParts = new();
        [ReadOnlyInspector] [SerializeField] private int currentCriticalLoses;

        public float MaxHp
        {
            get { return BodyParts.Sum(x => x.MaxHp); }
            set => throw new NotImplementedException("Уменьшить maxhp пропорционально по всем частям тела");
        }

        public PropertyManager Health { get; private set; }
        public float MaxWeightToGrab { get; set; }
        public event Action Died;

        /// <summary>
        /// Подписка на аналогичные ивенты у BodyParts 
        /// </summary>
        public event Action<float, float> HpChanged;

        /// <summary>
        /// Подписка на аналогичные ивенты у BodyParts 
        /// </summary>
        public event Action<float, Vector3> WhenDamagedOrHealed;


        public IReadOnlyList<BodyPart> BodyParts => bodyParts;

        public float Hp
        {
            get { return BodyParts.Sum(part => part.Hp); }
            set
            {
                var difference = value - BodyParts.Sum(part => part.Hp);
                if (Math.Abs(difference) < 0.001) return;
                var valuePart = 0f;
                if (difference < 0)//урон
                    valuePart = difference / BodyParts.Count(x => !x.IsDied);
                else // хил
                    valuePart = difference / BodyParts.Count(x => !x.IsDied && x.Hp != x.MaxHp);
                
                foreach (var bodyPart in bodyParts)
                    bodyPart.Hp += valuePart;
            }
        }

        public int MaxCriticalLoses => maxCriticalLoses;
        public bool IsDied => currentCriticalLoses >= maxCriticalLoses;

        protected virtual void Awake()
        {
            if (bodyPartRegisters.Count == 0)
                Debug.LogError("У тела нет частей тела!");

            Health = new PropertyManager(this);
            RegisterAllBodyParts();

        }

        protected virtual void OnEnable()
        {
            foreach (var bodyPart in bodyParts)
            {
                bodyPart.HpChanged += BodyPartOnHpChange;
                bodyPart.WhenDamagedOrHealed += BodyPartOnWhenDamagedOrHealed;
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var bodyPart in bodyParts)
            {
                bodyPart.HpChanged -= BodyPartOnHpChange;
                bodyPart.WhenDamagedOrHealed -= BodyPartOnWhenDamagedOrHealed;
            }
        }

        protected virtual void OnValidate()
        {
            foreach (var bodyPartRegister in bodyPartRegisters)
                bodyPartRegister.Validate();
            RegisterAllBodyParts();
        }

        private void RegisterAllBodyParts()
        {
            bodyParts.Clear();
            
            foreach (var bodyPartRegister in bodyPartRegisters)
                AddBodyPart(bodyPartRegister);

            if (bodyParts.Any(x => x == null))
                Debug.LogError("Часть тела не может быть null!");

            var count = bodyParts.Count;
            var distinctCount = bodyParts.Distinct().Count();
            if (count != distinctCount)
                Debug.LogError("Части тела дублируются!");
        }

        private void AddBodyPart(BodyPartRegister bodyPartRegister)
        {
            var bodyPart = bodyPartRegister.BodyPart;
            var significance = bodyPartRegister.Significance;

            if (bodyPart == null) throw new ArgumentException();
            bodyParts.Add(bodyPart);

            bodyPart.Died += () =>
            {
                if (IsDied) return;
                this.currentCriticalLoses += significance;
                if (currentCriticalLoses >= maxCriticalLoses)
                {
                    Died?.Invoke();
                }
            };
        }

        private void BodyPartOnWhenDamagedOrHealed(float dif, Vector3 pos)
        {
            WhenDamagedOrHealed?.Invoke(dif, pos);
        }

        private void BodyPartOnHpChange(float firstHp, float afterHp)
        {
            HpChanged?.Invoke(firstHp, afterHp);
        }

        public void FullHeal()
        {
            foreach (var bodyPart in bodyParts)
            {
                bodyPart.Hp = bodyPart.MaxHp;
            }
        }
        
        public virtual BodyState CreateState()
        {
            var x = Health.HealthProperties.ToArray();
            return new BodyState()
            {
                healthProperties = Health.HealthProperties.ToArray(),
                currentCriticalLoses = currentCriticalLoses,
                bodyPartSaves = bodyParts.Select(x => x.CreateState()).ToArray()
            };
        }

        public virtual void Restore(BodyState state, bool isLoadTo3D = false)
        {
            Health = new PropertyManager(this, state.healthProperties);
            currentCriticalLoses = state.currentCriticalLoses;
            for (int i = 0; i < bodyParts.Count; i++)
                bodyParts[i].Restore(state.bodyPartSaves[i]);
        }
    }
}