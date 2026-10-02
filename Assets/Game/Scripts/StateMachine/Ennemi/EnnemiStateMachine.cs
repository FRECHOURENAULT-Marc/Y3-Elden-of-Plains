using System.Collections.Generic;
using UnityEngine;

public class EnnemiStateMachine : IStateMachine
{
    [SerializeField] Transform _parent;
    [SerializeField] Animator _animator;
    [SerializeField] FollowTransform _followTransform;
    [SerializeField] WeaponScript _weapon;
    [SerializeField] float _attackRange;
    
    protected override void Start()
    {
        //States
        EnnemiFollowState follow = new EnnemiFollowState
        {
            followTransform = _followTransform
        };
        EnnemiAttackState attack = new EnnemiAttackState
        {
            weapon = _weapon,
            anim = _animator
        };

        //Transitions
        EnnemiFollowToAttack followToAtk = new EnnemiFollowToAttack
        {
            followTransform = _followTransform,
            nextState = attack
        };
        AddState(follow, followToAtk);
        
        EnnemiAttackToFollow atkToFollow = new EnnemiAttackToFollow
        {
            followTransform = _followTransform,
            weapon = _weapon,
            nextState = follow
        };
        AddState(attack, atkToFollow);
        
        //Define first state
        base.Start();
    }
}
