using UnityEngine;

public class EnnemiAttackState : IState
{
    public WeaponScript weapon;
    public Animator anim;

    public override void Execute()
    {
        if(weapon.IsAttackReady)
            anim.SetTrigger("Attack");
        
        weapon.Attack();
    }
    
}
