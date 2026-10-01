using System;
using UnityEngine;

[Serializable] public abstract class IState
{
    public virtual void Enter() {}
    public virtual void Execute() {}
    public virtual void Exit() {}
}
