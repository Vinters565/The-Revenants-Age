using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decision
{
    public FightState State { get; set;}
    public Transform Target { get; set;}

    public Decision (FightState state = FightState.EndTurnPhase, Transform target = null)
    {
        State = state;
        Target = target;
    }
}
