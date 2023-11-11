using System;
using UnityEngine;

namespace TheRevenantsAge
{
    public class BaseGlobalMapEntity: MonoBehaviour, IGlobalMapEntity
    {
        private BaseEntity entity;
        public Body Body => entity.Body;

        protected virtual void Awake()
        {
            entity = GetComponent<BaseEntity>();
        }

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            
        }
    }
}