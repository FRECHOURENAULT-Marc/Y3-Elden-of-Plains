using UnityEngine;

public class EnnemiAttackToFollow: ITransition
{
    public float attackRange;
    public WeaponScript weapon;
    public FollowTransform followTransform;
    
    public override bool Test()
    {
        return followTransform.GetDistanceToTarget() > attackRange
         && weapon.IsCurrentlyAttacking == false;
    }
}
