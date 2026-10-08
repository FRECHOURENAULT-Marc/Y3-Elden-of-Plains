using System;
using UnityEngine;
using EnnemiState;
using EnnemiTransition;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine.AI;
using UnityEngine.Animations;
using Deps = DependenciesInjector;

public class EnnemiHeavyStateMachine : IStateMachine
{
    [Header("Configuration")]
    [SerializeField] Transform _parent;
    [SerializeField] Animator _animator;
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] LivingEntity _life;
    [SerializeField] FollowTransform _followTransform;
    [SerializeField] WeaponScript _weapon;
    [SerializeField] ItemDrop _drop;
    [Header("Particules")]
    [Header("Death")]
    [SerializeField] ParticulePlayer _despawnPartPlayer;
    [SerializeField] ParticuleData _despawnPartData;    
    [Header("Heavy Attack")]
    [SerializeField] ParticulePlayer _heavyPartPlayer;
    [SerializeField] ParticuleData _heavyPartData;    
    [Header("Attack Impact")]
    [SerializeField] ParticulePlayer _impactPartPlayer;
    [SerializeField] ParticuleData _impactPartData;
    [SerializeField] CinemachineImpulseSource _impactImpulse;
    
    [Button]
    void TryGetConfiguration()
    {
        if(_agent == null)
            _agent = _parent.gameObject.GetComponent<NavMeshAgent>();
        if(_life == null)
            _life = _parent.gameObject.GetComponent<LivingEntity>();
        if(_weapon == null)
            _weapon = _parent.gameObject.GetComponentInChildren<WeaponScript>();
    }
    
    protected override void Start()
    {
        _life.OnDamaged += OnHit;
        _life.OnDeath += OnDeath;
        
        UpdateSpeedAnim();

        CreateStateMachine();
        
        //Define first state
        base.Start();
    }

    private void OnDestroy()
    {
        _life.OnDamaged -= OnHit;
        _life.OnDeath -= OnDeath;
    }

    void OnHit()
    {
        _weapon.CancelAttack();
        _animator.SetTrigger("Hit");
    }
    async void OnDeath()
    {
        _agent.isStopped = true;
        _animator.SetTrigger("Death");
        await Awaitable.WaitForSecondsAsync(2, destroyCancellationToken);
        if (destroyCancellationToken.IsCancellationRequested)
            return;
        _despawnPartPlayer.PlayEffect(_despawnPartData.name);
        
        _drop.DropItem(transform.position + Vector3.up);
        Destroy(_parent.gameObject);
        Debug.Log(_parent.gameObject.name + " was destroyed");
    }
    
    void CreateStateMachine()
    {
        // States
        EnnemiState.Idle idle = new();
        EnnemiState.Follow follow = new();
        EnnemiState.AttackHeavy attack = new AttackHeavy
        {
            followTransform = _followTransform,
            weapon = _weapon,
            anim = _animator,
            life = _life,
            partHeavyPlayer = _heavyPartPlayer,
            partHeavyData = _heavyPartData,
            partImpactPlayer = _impactPartPlayer,
            partImpactData = _impactPartData,
            impulseImpact = _impactImpulse
        };

        // Transitions
        // from idle
        IdleToFollow idleToFollow = new IdleToFollow
        {
            followTransform = _followTransform,
            nextState = follow,
        };
        AddState(idle, idleToFollow);
        // from follow
        FollowToIdle followToIdle = new FollowToIdle
        {
            followTransform = _followTransform,
            nextState = idle
        };
        FollowToAttack followToAtk = new FollowToAttack
        {
            followTransform = _followTransform,
            nextState = attack
        };
        AddState(follow, followToIdle, followToAtk);
        // from atk
        AttackToFollow atkToFollow = new AttackToFollow
        {
            followTransform = _followTransform,
            weapon = _weapon,
            nextState = follow
        };
        AddState(attack, atkToFollow);
    }
    
    async void UpdateSpeedAnim()
    {
        while (destroyCancellationToken.IsCancellationRequested == false)
        {
            _animator.SetFloat("Speed", _agent.velocity.magnitude);
            await Awaitable.NextFrameAsync(destroyCancellationToken);
        }
            
    }
}
