using UnityEngine;

public class EnnemiFollowToAttack : ITransition
{
    public float attackRange;
    public FollowTransform followTransform;
    
    public override bool Test()
    {
        return followTransform.GetDistanceToTarget() < attackRange;
    }
}
