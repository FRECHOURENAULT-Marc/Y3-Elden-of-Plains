using System;
using UnityEngine;
using EnnemiState;
using EnnemiTransition;
using NaughtyAttributes;
using UnityEngine.AI;
using UnityEngine.Animations;
using Deps = DependenciesInjector;

public class EnnemiStateMachine : IStateMachine
{
    [Header("Configuration")]
    [SerializeField] Transform _parent;
    [SerializeField] Animator _animator;
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] LivingEntity _life;
    [SerializeField] FollowTransform _followTransform;
    [SerializeField] WeaponScript _weapon;
    [SerializeField] ItemDrop _drop;
    [SerializeField] float _counterTime;
    [Header("Particules")]
    [SerializeField] ParticulePlayer _counterPartPlayer;
    [SerializeField] ParticuleData _counterPartData;    
    [SerializeField] ParticulePlayer _despawnPartPlayer;
    [SerializeField] ParticuleData _despawnPartData;

    Parade _playerParry;
    
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
        _playerParry = Deps.PDeps.Parry;
        
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
        EnnemiState.Follow follow = new();
        EnnemiState.Attack attack = new Attack
        {
            weapon = _weapon,
            anim = _animator
        };
        EnnemiState.Counter counter = new Counter
        {
            weapon = _weapon,
            anim = _animator,
            partPlayer = _counterPartPlayer,
            partData = _counterPartData,
            counterTime = _counterTime
        };

        // Transitions
        // from follow
        FollowToAttack followToAtk = new FollowToAttack
        {
            followTransform = _followTransform,
            nextState = attack
        };
        AddState(follow, followToAtk);
        // from atk
        AttackToFollow atkToFollow = new AttackToFollow
        {
            followTransform = _followTransform,
            weapon = _weapon,
            nextState = follow
        };
        AttackToCounter atkToCounter = new AttackToCounter
        { 
            weapon = _weapon, 
            parry = _playerParry,
            nextState = counter,
        };
        atkToCounter.Init();
        AddState(attack, atkToFollow, atkToCounter);
        // from counter
        CounterToFollow counterToFollow = new CounterToFollow
        {
            counter = counter,
            nextState = follow
        };
        AddState(counter, counterToFollow);
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
