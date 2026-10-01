using UnityEngine;

public class EnnemiAttackToFollow: ITransition
{
    public float attackRange;
    public FollowTransform followTransform;
    public WeaponScript weapon;
    
    public override bool Test()
    {
        return followTransform.GetDistanceToTarget() > attackRange
         && weapon.IsCurrentlyAttacking == false;
    }
}
