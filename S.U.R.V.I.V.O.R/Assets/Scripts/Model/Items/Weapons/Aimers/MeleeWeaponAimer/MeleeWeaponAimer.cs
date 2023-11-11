using System.Collections;
using System.Collections.Generic;
using TheRevenantsAge;
using UnityEngine;

public class MeleeWeaponAimer: Aimer
{
    public override void ChangeCameraToAimMode()
    {
    }
    
    public override void ChangeCameraPosition(Transform characterObj)
    {
    }
    
    
    public override void DrawAimLine(Vector3 aimPoint)
    {
    }
    
    public override Vector3 Aim()
    {
        return Vector3.zero;
    }
    
    public override void StopDrawAiming()
    {
    }
}
