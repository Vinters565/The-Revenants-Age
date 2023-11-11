using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using UnityEngine;

public class ActionBTNode : BaseBTNode
{
    //Лист Behaviour Tree. Выполняет только действие
    public ActionBTNode(string name, Func<Decision, AIInformation, BTNodeStatus> action) : base(name, action)
    {
    }

    public override BTNodeStatus Enter(Decision decision, AIInformation information)
    {
        if (CurrentStatus != BTNodeStatus.Unknown)
            throw new CompositionException("Цикл в BehaviourTree: " + Name);
        CurrentStatus = EnterAction.Invoke(decision, information);
        return CurrentStatus;
    }
}
