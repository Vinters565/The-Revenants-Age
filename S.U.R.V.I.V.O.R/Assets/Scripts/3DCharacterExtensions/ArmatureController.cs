using System.Collections.Generic;
using Extension;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheRevenantsAge
{
    public class ArmatureController : MonoBehaviour
    {
        [SerializeField] private Transform armature;
        private List<Collider> colliders = new();
        private List<Rigidbody> rigidbodies = new();

        private void Awake()
        {
            foreach (var child in armature.IterateByAllChildren())
            {
                var collider = child.GetComponent<Collider>();
                var rigidBody = child.GetComponent<Rigidbody>();
                if(collider)
                    colliders.Add(collider);
                if(rigidBody)
                    rigidbodies.Add(rigidBody);
            }
        }

        public void SetActiveAllColliders(bool value)
        {
            foreach (var collider in colliders)
            {
                collider.enabled = value;

            }
        }

        public void SetActiveAllRigidBodies(bool value)
        {
            foreach (var rigidBody in rigidbodies)
            {
                rigidBody.useGravity = value;
            }
        }
    }
}
