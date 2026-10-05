using System;
using UnityEngine;

[Serializable] public abstract class ITransition
{
    public IState nextState;
    public virtual void Init() {}
    
    public abstract bool Test();
    
}
