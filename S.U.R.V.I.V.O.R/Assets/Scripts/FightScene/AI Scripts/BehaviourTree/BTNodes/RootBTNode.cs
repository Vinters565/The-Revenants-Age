using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Remoting.Services;
using UnityEngine;

public class RootBTNode : BaseBTNode
{
    //Корневой элемент в BehaviourTree. Создает объект класса Decision
    public RootBTNode(string name, Func<Decision, AIInformation, BTNodeStatus> action) : base(name, action) {}

    public Decision MakeDecision(AIInformation information)
    {
        var decision = new Decision();
        if (Enter(decision, information) == BTNodeStatus.Succeed)
            return decision;
        return new Decision();

    }

    public override BTNodeStatus Enter(Decision decision, AIInformation information)
    {
        if (CurrentStatus != BTNodeStatus.Unknown)
            throw new CompositionException("Цикл в BehaviourTree: " + Name);
        foreach (var child in Children)
        {
            if (child.Enter(decision, information) == BTNodeStatus.Succeed)
            {
                CurrentStatus = BTNodeStatus.Succeed;
                return BTNodeStatus.Succeed;
            }
        }

        CurrentStatus = BTNodeStatus.Failed;
        return BTNodeStatus.Failed;
    }
}
