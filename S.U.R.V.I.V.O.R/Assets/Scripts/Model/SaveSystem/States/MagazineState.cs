using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace TheRevenantsAge
{
    [DataContract]
    public class MagazineState : ComponentState
    {
        [DataMember] public string[] ammoResourcesPaths;

        public IEnumerable<SingleAmmo> AmmoScriptableObjects => ammoResourcesPaths.Select(Resources.Load<SingleAmmo>);
    }
}