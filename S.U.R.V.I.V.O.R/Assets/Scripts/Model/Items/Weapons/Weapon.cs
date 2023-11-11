using UnityEngine;
using UnityEngine.Events;

namespace TheRevenantsAge
{
    [RequireComponent(typeof(BaseItem))]
    public abstract class Weapon : MonoBehaviour
    {
        protected BaseItem baseItem;
        public abstract Aimer Aimer { get; }
        public abstract float Ergonomics { get; }
        public virtual bool IsActive { get; set; }

        public UnityEvent WhenAttacked;

        /// <summary>
        ///  Синтаксический сахар, чтобы не обращаться к BaseItem
        /// </summary>
        public ICharacter Owner
        {
            get => baseItem.ItemOwner;
            set => baseItem.ItemOwner = value;
        }

        public abstract HandlingTypes HandlingType { get; }
        public abstract void Attack(Vector3 targetPoint, CharacterSkills skills);


        protected virtual void Awake()
        {
            baseItem = GetComponent<BaseItem>();
        }

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            
        }
    }
}