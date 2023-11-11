using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimation : WeaponAnimation
{
    private const string ATTACK_TRIGGER = "Attack";
    private GunMuzzleFlash muzzleFlash;
    private Animator animator;
    private Queue<Vector3> hitPoints;
    private void Awake()
    {
        muzzleFlash = GetComponent<GunMuzzleFlash>();
        animator = GetComponent<Animator>();
        hitPoints = new Queue<Vector3>();
    }

    public void OnAttack(Vector3 hitPoint)
    {
        hitPoints.Enqueue(hitPoint);
        animator.SetTrigger(ATTACK_TRIGGER);
        Debug.Log("ATTACK");
    }

    public void GUN_ANIMATION_MUZZLE_FLASH()
    {
        muzzleFlash.StartMuzzleFlash();
    }
    
    public void GUN_ANIMATION_BULLET_TRACER()
    {
        muzzleFlash.MakeShootTracer(hitPoints.Dequeue());
    }
}
