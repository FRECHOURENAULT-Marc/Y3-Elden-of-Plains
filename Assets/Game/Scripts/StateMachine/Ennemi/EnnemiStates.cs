using UnityEngine;
///// STATES
namespace EnnemiState {
public class Follow : IState { }
public class Attack : IState
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
public class Counter : IState
{
    public WeaponScript weapon;
    public Animator anim;
    public float counterTime;
    private bool _isEnded;

    public bool IsEnded
    {
        get => _isEnded;
    } 
    
    public override void Enter()
    {
        anim.SetTrigger("Counter");
        weapon.CancelAttack();
        _isEnded = false;
        HandleCounter();
    }
    
    async void HandleCounter()
    {
        await Awaitable.WaitForSecondsAsync(counterTime);
        _isEnded = true;
    }
}

};
