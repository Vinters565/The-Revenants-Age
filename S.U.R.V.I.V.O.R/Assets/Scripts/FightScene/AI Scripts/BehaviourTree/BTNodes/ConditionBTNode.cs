using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using UnityEngine;

public class ConditionBTNode : BaseBTNode
{
    //Выполняет действие, если успешно выполнены все потомки
    public ConditionBTNode(string name, Func<Decision, AIInformation, BTNodeStatus> action) : base(name, action) {}
    public override BTNodeStatus Enter(Decision decision, AIInformation information)
    {
        if (CurrentStatus != BTNodeStatus.Unknown)
            throw new CompositionException("Цикл в BehaviourTree: " + Name);
        var succeedAll = true;
        foreach (var child in Children)
        {
            succeedAll &= child.Enter(decision, information) == BTNodeStatus.Succeed;
        }

        CurrentStatus = succeedAll ? BTNodeStatus.Succeed : BTNodeStatus.Failed; 
        return succeedAll ? EnterAction.Invoke(decision, information) : BTNodeStatus.Failed;
    }
}
