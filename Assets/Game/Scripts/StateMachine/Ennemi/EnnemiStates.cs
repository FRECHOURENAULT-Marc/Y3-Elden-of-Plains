using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

///// STATES
namespace EnnemiState { 
public class Idle : IState { }
public class Follow : IState { }
public class Attack : IState
{
    public FollowTransform followTransform;
    public WeaponScript weapon;
    public Animator anim;

    protected bool _hasAttacked = false;
    
    public override void Enter()
    {
        followTransform.StopFollowingForce();
    }       
    public override void Exit()
    {
        followTransform.ResumeFollowing();
    }    
    
    public override void Execute()
    {
        if (_hasAttacked && weapon.IsRecovering)
        {
            _hasAttacked = false;
            followTransform.ResumeFollowing();
        }
        if (weapon.IsAttackReady)
        {
            anim.SetTrigger("Attack");
            _hasAttacked = true;
        }
        
        weapon.Attack();
    }
}
public class AttackHeavy : Attack
{
    public LifeScript life;
    public ParticulePlayer partHeavyPlayer;
    public ParticuleData partHeavyData;    
    public ParticulePlayer partImpactPlayer;
    public ParticuleData partImpactData;
    public CinemachineImpulseSource impulseImpact;

    public override void Execute()
    {
        if (life.IsAlive == false) // Destroy soon
            return;
        
        if (_hasAttacked && weapon.IsRecovering)
        {
            impulseImpact.GenerateImpulse();
            partImpactPlayer.PlayEffect(partImpactData.name, partImpactPlayer.transform.position, partImpactData.scale);
            _hasAttacked = false;
            followTransform.ResumeFollowing();
        }
        if (weapon.IsAttackReady)
        {
            partHeavyPlayer.PlayEffect(partHeavyData.name, partHeavyPlayer.transform.position, partHeavyData.scale);
            anim.SetTrigger("Attack");
            _hasAttacked = true;
        }
        
        weapon.Attack();
    }
}

public class Counter : IState
{
    public WeaponScript weapon;
    public Animator anim;
    public ParticulePlayer partPlayer;
    public ParticuleData partData;
    public float counterTime;
    private bool _isEnded;

    public bool IsEnded
    {
        get => _isEnded;
    } 
    
    public override void Enter()
    {
        anim.SetTrigger("Counter");
        partPlayer.PlayEffect(partData.name, partPlayer.transform.position, partData.scale);
        partPlayer.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
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
