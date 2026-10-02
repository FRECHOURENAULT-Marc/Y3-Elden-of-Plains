using UnityEngine;

public class EnnemiAttackToFollow: ITransition
{
    public FollowTransform followTransform;
    public WeaponScript weapon;
    
    public override bool Test()
    {
        return followTransform.IsStopped == false
         && weapon.IsCurrentlyAttacking == false;
    }
}
