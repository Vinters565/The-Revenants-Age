using System;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    [Serializable]
    public class BodyPartRegister
    {
        [SerializeField] [HideInInspector] private string title;
        
        [field: SerializeField]
        public BodyPart BodyPart { get; private set; }

        [field: SerializeField]
        [field: Min(0)]
        public int Significance { get; private set; } = 1;

        public void Validate()
        {
            if (BodyPart is null)
                title = "No BodyPart";
            else if (BodyPart == null)
                title = "Missing Reference";
            else
                title = $"{BodyPart.name} x{Significance}";
        }
    }
}