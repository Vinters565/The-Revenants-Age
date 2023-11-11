using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Extension;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public class BodyPart : MonoBehaviour, IAlive
    {
        private Body body;
        public PropertyManager Health { get; private set; }
        [ReadOnlyInspector] [SerializeField]private float hp;
        
        [SerializeField][Min(1)] float maxHp = 100;
        [SerializeField][Min(1)] private float size = 100;
        [SerializeField] [HideInInspector] private List<BodyPart> neighborBodyParts = new ();

        private readonly List<Transform> pointersTransforms = new ();

        private bool hpChanged;
        private float differanceInHp; 
        
        /// <summary>
        /// Вызывается, когда здоровье части тела становится равным нулю.
        /// </summary>
        public event Action Died;
        
        /// <summary>
        /// Первое значение - количество хп до изменения, второе - хп после изменения.
        /// Вызывается мгновенно.
        /// </summary>
        public event Action<float, float> HpChanged;
        
        /// <summary>
        /// Первое значение - урон, если меньше 0, или хил, если больше,
        /// второе - место расположение.
        /// Вызывается отложенно.
        /// </summary>
        public event Action<float, Vector3> WhenDamagedOrHealed;

        public Body Body
        {
            get
            {
                if (body is not null)
                    return body;
                
                var tempRef = transform.parent.GetComponent<Body>();
                if (tempRef is not null)
                    return tempRef;
                throw new CompositionException("Нарушина иерархия! Body->BodyPart");
            }
        }

        public IReadOnlyList<BodyPart> GetNeighborBodyParts => neighborBodyParts;

        public void AddNeighbor(BodyPart bodyPart)
        {
            if (this.Body == bodyPart.Body)
                neighborBodyParts.Add(bodyPart);
            else
                throw new InvalidOperationException();
        }
        
        public void RemoveNeighbor(BodyPart bodyPart)
        {
            neighborBodyParts.Remove(bodyPart);
        }

        public float MaxHp
        {
            get => maxHp;
            set
            {
                value = Math.Max(1, value);
                var multiplier = value / maxHp;
                maxHp = value;
                Hp *= multiplier;
            }
        }

        public float Size
        {
            get => size;
            set => size = Math.Max(1, value);
        }

        public float Hp
        {
            get => hp;
            set
            {
                var before = hp;   
                if (value <= 0.01f)
                {
                    if (IsDied)
                    {
                        DamageAllNeighbors(before - value);
                        return;
                    };
                    value = 0;
                }
                
                hp = Math.Min((float)Math.Round(value, 1),MaxHp); // чтобы не привысить максимум
                HpChanged?.Invoke(before, hp);

                // для визуализации, необходимо отложенный вызов ивента
                hpChanged = true;
                differanceInHp += hp - before;
                //
                
                if (hp < 0.01f)
                {
                    Died?.Invoke();
                }
            }
        }

        private void DamageAllNeighbors(float damage)
        {
            var visited = new HashSet<BodyPart>();
            var targets = new List<BodyPart>();
            var queue = new Queue<BodyPart>();

            queue.Enqueue(this);
            visited.Add(this);

            while (queue.Count != 0)
            {
                var part = queue.Dequeue();
                foreach (var neighbor in part.neighborBodyParts)
                {
                    if (visited.Contains(neighbor)) continue;
                    visited.Add(neighbor);

                    if (neighbor.IsDied)
                        queue.Enqueue(neighbor);
                    else
                        targets.Add(neighbor);
                }
            }

            var distributedDamage = damage / targets.Count;

            foreach (var target in targets)
                target.Hp -= distributedDamage;
        }

        public bool IsDied => Hp <= 0;

        protected virtual void Awake()
        {
            Health = new PropertyManager(this);
            hp = maxHp;

            body = transform.parent?.GetComponent<Body>();
            if (body == null )
                throw new CompositionException("Нарушина иерархия! Body->BodyPart");
        }

        protected virtual void LateUpdate()
        {
            if (hpChanged)
            {
                hpChanged = false;
                WhenDamagedOrHealed?.Invoke(differanceInHp, GetMyPosition());
                differanceInHp = 0;
            }
        }

        protected Vector3 GetMyPosition()
        {
            if (pointersTransforms.Count == 0)
                return transform.position;
            if (pointersTransforms.Count == 1)
                return pointersTransforms[0].position;
            var currentPos = pointersTransforms[0].position;
            for (int i = 1; i < pointersTransforms.Count; i++)
                currentPos = (currentPos + pointersTransforms[i].position) / 2;
            return currentPos;
        }

        public void RegisterPointerToBodyPart(PointerToBodyPart pointerToBodyPart)
        {
            if (pointerToBodyPart.BodyPart != this)
                throw new InvalidOperationException();
            pointersTransforms.Add(pointerToBodyPart.transform);
        }

        public virtual BodyPartState CreateState()
        {
            return new BodyPartState()
            {
                healthProperties = Health.HealthProperties.ToArray(),
                maxHp = MaxHp,
                hp = Hp,
                size = Size
            };
        }

        public virtual void Restore(BodyPartState state)
        {
            Health = new PropertyManager(this, state.healthProperties);
            maxHp = state.maxHp;
            hp = state.hp;
            size = state.size;
        }

        private void OnValidate()
        {
            hp = maxHp;
        }
    }
}