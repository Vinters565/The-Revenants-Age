using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ConstraintController : MonoBehaviour
{
    [SerializeField] private List<MultiAimConstraint> aimConstraints;
    [SerializeField] private TwoBoneIKConstraint leftHandBoneConstraint;
    [SerializeField] private TwoBoneIKConstraint rightHandBoneConstraint;
    
    private RigBuilder builder;
    

    private void Awake()
    {
        builder = GetComponent<RigBuilder>();
    }

    public void SetTargetForMultiAimConstraints(Transform target)
    {
        foreach (var con in aimConstraints)
        {
            con.data.sourceObjects = new WeightedTransformArray{ new WeightedTransform(target.transform, 1f)};
        }
    }

    public void SetTargetForRightHand(Transform target)
    {
        rightHandBoneConstraint.data.target = target;
    }
    
    public void SetTargetForLeftHand(Transform target)
    {
        leftHandBoneConstraint.data.target = target;
    }

    public void RebuildRig()
    {
        builder.Build();
    }
}
