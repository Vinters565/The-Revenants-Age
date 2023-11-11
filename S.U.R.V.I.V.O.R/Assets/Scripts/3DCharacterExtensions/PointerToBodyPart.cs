using System;
using System.Data;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public class PointerToBodyPart : MonoBehaviour
    {
        public IEntity MyEntity { get; private set; }
        [field: SerializeField] public BodyPart BodyPart { get; private set; }

        private void Awake()
        {
            if (BodyPart is null)
                throw new ConstraintException("Invalid!");
            BodyPart.RegisterPointerToBodyPart(this);
            FindMyEntity();
        }

        private void FindMyEntity()
        {
            var currentTransform = transform;
            while (currentTransform)
            {
                var entity = currentTransform.GetComponent<IEntity>();
                if (entity is not null)
                {
                    MyEntity = entity;
                    break;
                }

                currentTransform = currentTransform.parent;
            }
        }
    }
}
