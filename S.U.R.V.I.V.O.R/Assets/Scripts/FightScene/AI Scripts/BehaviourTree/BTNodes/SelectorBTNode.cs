using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using UnityEngine;

public class SelectorBTNode : BaseBTNode
{
    //Все потомки выполняются по порядку добавления.
    //Выполняет действие при первом успешно выполненом потомке
    public SelectorBTNode(string name, Func<Decision, AIInformation, BTNodeStatus> action) : base(name, action) {}
    public override BTNodeStatus Enter(Decision decision, AIInformation information)
    {
        if (CurrentStatus != BTNodeStatus.Unknown)
            throw new CompositionException("Цикл в BehaviourTree: " + Name);
        foreach (var child in Children)
        {
            if (child.Enter(decision, information) == BTNodeStatus.Succeed)
            {
                CurrentStatus = BTNodeStatus.Succeed;
                return EnterAction(decision, information);
            }
        }

        CurrentStatus = BTNodeStatus.Failed;
        return BTNodeStatus.Failed;
    }
}
