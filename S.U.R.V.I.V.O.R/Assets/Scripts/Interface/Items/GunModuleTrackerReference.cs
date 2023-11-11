using UnityEngine;

namespace Interface.Items
{
    public class GunModuleTrackerReference : MonoBehaviour
    {
        [SerializeField] private GunModuleTracking gunModuleTracking;
        public GunModuleTracking GunModuleTracking => gunModuleTracking;
    }
}