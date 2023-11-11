using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourTree: MonoBehaviour
{
    public virtual RootBTNode Root { get; }
}
