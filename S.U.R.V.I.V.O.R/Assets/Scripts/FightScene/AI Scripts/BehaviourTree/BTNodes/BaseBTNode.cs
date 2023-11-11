using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseBTNode
{
    public string Name { get; private set; }
    public BTNodeStatus CurrentStatus { get; protected set; } = BTNodeStatus.Unknown;

    protected List<BaseBTNode> Children = new ();
    protected Func<Decision, AIInformation, BTNodeStatus> EnterAction { get; private set;}

    public BaseBTNode(string name, Func<Decision, AIInformation, BTNodeStatus> action)
    {
        Name = name;
        EnterAction = action;
    }

    public BaseBTNode AddChild(BaseBTNode child)
    {
        Children.Add(child);
        return child;
    }

    public void Reset()
    {
        CurrentStatus = BTNodeStatus.Unknown;
        foreach (var child in Children)
            child.Reset();
    }
    public virtual BTNodeStatus Enter(Decision decision, AIInformation information)
    {
        return EnterAction.Invoke(decision, information);
    }
}
