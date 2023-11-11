using System;
using TheRevenantsAge;
using Extension;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheRevenantsAge
{
    public abstract class WeaponAnimator : MonoBehaviour
    {
        [field: SerializeField] public AnimatorOverrideController Animations { get; private set; }
        
        [SerializeField] [HideInInspector] protected Vector3 offsetLocalPosition;
        [SerializeField] [HideInInspector] protected Vector3 offsetLocalRotation;
        

        private void Awake()
        {
            //AddSelfToAnimationController();
            ChangeWeaponOffsetToDefault();
        }

        public abstract void OnTake();

        public void ValidatePositionAndRotation()
        {
            var parent = transform.parent;
            if (parent != null)
            {
                offsetLocalPosition = parent.localPosition.Round(4);
                offsetLocalRotation = parent.localRotation.eulerAngles.Round(4);
            }
        }

        public void ChangeWeaponOffsetToDefault()
        {
            var parent = transform.parent;
            if (parent)
            {
                parent.localPosition = offsetLocalPosition;
                parent.localRotation = Quaternion.Euler(offsetLocalRotation);
            }
        }
    }
}