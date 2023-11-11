using System;
using System.ComponentModel.Composition;
using DataBase;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    [RequireComponent(typeof(Pointer))]
    public class BaseEntity: MonoBehaviour, IEntity
    {
        [SerializeField] private Body body;
        public Body Body => body;

        protected virtual void Awake()
        {
            if (Body == null)
                throw new CompositionException("Invalid!");
        }

        protected virtual void OnEnable()
        {}

        protected virtual void OnDisable()
        {}
    }
}