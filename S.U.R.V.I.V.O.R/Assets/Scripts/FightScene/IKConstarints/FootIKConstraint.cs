using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class FootIKConstraint : MonoBehaviour
{
    [Header("Raycast settings")]
    [SerializeField] [Range(0, 2f)] private float maxRaycastDistance = 1;
    [SerializeField] [Range(0, 3f)] private float maxHeadRaycastDistance = 2;
    [SerializeField] [Range(0, 3f)] private float maxBodyRaycastDistance = 2;
    
    [Header("Height settings")]
    [SerializeField] [Range(0, 2f)] private float allBodyHeight = 1;
    [SerializeField] [Range(0, 2f)] private float headHeight = 1;
    [SerializeField] [Range(0, 1f)] private float crouchRate = 0;

    [Space(10)]
    [Header("RayCastStarts")]
    [SerializeField] private Transform leftForwardFootStart;
    [SerializeField] private Transform rightForwardFootStart;
    [SerializeField] private Transform leftBackFootStart;
    [SerializeField] private Transform rightBackFootStart;
    [SerializeField] private Transform allBodyStart;
    [SerializeField] private Transform headStart;

    [Space(10)] 
    [Header("Hints")] 
    [SerializeField] private Transform leftForwardHint;
    [SerializeField] private Transform rightForwardHint;
    [SerializeField] private Transform leftBackHint;
    [SerializeField] private Transform rightBackHint;
    
    [Space(10)]
    [Header("Targets")]
    [SerializeField] private Transform leftForwardTarget;
    [SerializeField] private Transform rightForwardTarget;
    [SerializeField] private Transform leftBackTarget;
    [SerializeField] private Transform rightBackTarget;
    [SerializeField] private Transform allBodyTarget;
    [SerializeField] private Transform headTarget;

    [Space(10)] [Header("Other links")] [SerializeField]
    private Rig legsRig;

    private Animator animator;

    private Transform leftFootBone;
    private Transform rightFootBone;
    
    private Vector3 bufferLeftFootPos;
    private Vector3 bufferRightFootPos;

    private Transform leftMark;
    private Transform rightMark;
    

    private void Awake()
    {
        leftMark = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        leftMark.localScale = Vector3.one * 0.2f;
        //animator = GetComponent<Animator>();
        // leftFootBone = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        // rightFootBone = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        // animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        // animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
    }

    private void Update()
    {
        RaycastFromBones();
    }

    // private void OnAnimatorIK(int layerIndex)
    // {
    //     animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
    //     animator.SetIKPosition(AvatarIKGoal.LeftFoot, bufferLeftFootPos);
    //     animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
    //     animator.SetIKPosition(AvatarIKGoal.RightFoot, bufferRightFootPos);
    // }
    

    // private Transform MarkBone(Transform bone)
    // {
    //     var mark = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
    //     mark.localScale = Vector3.one * 0.2f;
    //     return mark;
    // }

    private void RaycastFromBones()
    {
        var mask = 1 << LayerMask.NameToLayer("FightSceneCround");
        var leftForwardFootRay = new Ray(leftForwardFootStart.position, Vector3.down);
        var leftBackFootRay = new Ray(leftBackFootStart.position, Vector3.down);
        var rightForwardFootRay = new Ray(rightForwardFootStart.position, Vector3.down);
        var rightBackFootRay = new Ray(rightBackFootStart.position, Vector3.down);
        
        var headRay = new Ray(headStart.position, Vector3.down);
        var allBodyRay = new Ray(allBodyStart.position, Vector3.down);

        var successAllBodyHit =
            Physics.Raycast(allBodyRay, out var allBodyHit, maxBodyRaycastDistance, layerMask: mask);

        var successHeadHit = Physics.Raycast(headRay, out var headHit, maxHeadRaycastDistance, layerMask: mask);

        var resultAllBodyHeight = allBodyHeight;
        var resultHeadHeight = headHeight;
        
        if (successAllBodyHit && successHeadHit)
        {
            if (allBodyHit.point.y > headHit.point.y && legsRig.weight >= 0.1f)
            {
                resultAllBodyHeight -= (allBodyHit.point.y - headHit.point.y) * crouchRate;
            }
        }

        if (successAllBodyHit)
        {
            allBodyTarget.position = allBodyHit.point + Vector3.up * resultAllBodyHeight;
        }
        if (successHeadHit)
        {
            headTarget.position = headHit.point + Vector3.up * resultHeadHeight;
        }

        if (Physics.Raycast(leftForwardFootRay, out var leftForwardHit, maxRaycastDistance, layerMask: mask))
        {
            leftForwardTarget.position = leftForwardHit.point;
        }
        if (Physics.Raycast(leftBackFootRay, out var leftBackHit, maxRaycastDistance, layerMask: mask))
        {
            leftBackTarget.position = leftBackHit.point;
        }
        if (Physics.Raycast(rightForwardFootRay, out var rightHit, maxRaycastDistance, layerMask: mask))
        {
            rightForwardTarget.position = rightHit.point;
        }
        if (Physics.Raycast(rightBackFootRay, out var rightBackHit, maxRaycastDistance, layerMask: mask))
        {
            rightBackTarget.position = rightBackHit.point;
        }
    }
}
