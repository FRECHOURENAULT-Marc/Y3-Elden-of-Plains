using UnityEngine;

public class EnnemiFollowToAttack : ITransition
{
    public FollowTransform followTransform;
    
    public override bool Test()
    {
        return followTransform.IsStopped;
    }
}
