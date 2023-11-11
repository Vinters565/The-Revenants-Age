using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using TheRevenantsAge;
using UnityEngine;

public class GunAnimator : WeaponAnimator
{
    private ConstraintController constraintController;
    private GunAimer aimer;
    private Transform leftHandParent;
    private Transform rightHandParent;

    private void Start()
    {
        aimer = GetComponent<GunAimer>();
        leftHandParent = transform.Find("LeftHandParent");
        rightHandParent = transform.Find("RightHandParent");
        Transform controller = transform;
        while (controller && !controller.GetComponent<ConstraintController>())
        {
            controller = controller.parent;
        }

        //if (constraintController is null)
        //    throw new CompositionException("Не обнаружен компонент ConstraintController");
        //else
        constraintController = controller.GetComponent<ConstraintController>();
    }

    public override void OnTake()
    {
        Debug.Log("RebuildRig");
        aimer.AimPoint.SetActive(true);
        constraintController.SetTargetForMultiAimConstraints(aimer.AimPoint.transform);
        aimer.AimPoint.SetActive(false);
        constraintController.SetTargetForRightHand(rightHandParent);
        constraintController.SetTargetForLeftHand(leftHandParent);
        constraintController.RebuildRig();
    }
}
