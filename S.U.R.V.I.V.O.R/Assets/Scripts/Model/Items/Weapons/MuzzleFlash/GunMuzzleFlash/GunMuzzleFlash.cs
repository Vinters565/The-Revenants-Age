using System.Collections;
using System.Collections.Generic;
using TheRevenantsAge;
using UnityEngine;

public class GunMuzzleFlash : MuzzleFlash
{
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject bulletTracer;
    [SerializeField] private Transform gunPoint;


    public override void StartMuzzleFlash()
    {
        muzzleFlash.Emit(1);
    }

    public override void MakeShootTracer(Vector3 hitPoint)
    {
        var bullet = Instantiate(bulletTracer, gunPoint.position, Quaternion.identity);
        var trail = bullet.GetComponent<TrailRenderer>();
        trail.AddPosition(gunPoint.position);
        trail.transform.position = hitPoint;
        bullet.transform.position = hitPoint;
    }

}
