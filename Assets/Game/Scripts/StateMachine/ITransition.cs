using System;
using UnityEngine;

[Serializable] public abstract class ITransition
{
    public IState nextState;

    public abstract bool Test();
}
