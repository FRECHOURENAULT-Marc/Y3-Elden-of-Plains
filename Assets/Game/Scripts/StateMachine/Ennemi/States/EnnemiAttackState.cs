using UnityEngine;

public class EnnemiAttackState : IState
{
    public WeaponScript weapon;

    public override void Execute()
    {
        weapon.Attack();
    }
    
}
