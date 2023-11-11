
using UnityEngine;

namespace TheRevenantsAge
{
    public abstract class Aimer: MonoBehaviour
    {
        public FightCameraMode AimMode { get; }

        public abstract void ChangeCameraToAimMode();

        public abstract void ChangeCameraPosition(Transform characterObj);

        public abstract void DrawAimLine(Vector3 aimPointPos);
        
        public abstract Vector3 Aim();

        public abstract void StopDrawAiming();

    }
}