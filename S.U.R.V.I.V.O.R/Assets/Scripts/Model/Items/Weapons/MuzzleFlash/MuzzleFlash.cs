using UnityEngine;

namespace TheRevenantsAge
{
    public abstract class MuzzleFlash: MonoBehaviour
    {
        public abstract void StartMuzzleFlash();

        public abstract void MakeShootTracer(Vector3 hitPoint);
    }
}
