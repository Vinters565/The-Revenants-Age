using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    [DataContract]
    public class AmmoBoxState : ComponentState
    {
        [DataMember] public string[] ammoResourcesPaths;
        public IEnumerable<SingleAmmo> AmmoScriptableObjects => ammoResourcesPaths
            .Select(Resources.Load<SingleAmmo>);
    }
}
