using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Behaviour : MonoBehaviour
{
    [SerializeField] private BehaviourTree behaviourTree;

    // private void Awake()
    // {
    //     behaviourTree = (DogBehaviourTree)behaviourTreeObject;
    // }

    public Decision MakeDecision(AIInformation aiInformation)
    {
        behaviourTree.Root.Reset();
        return behaviourTree.Root.MakeDecision(aiInformation);
    }
}
