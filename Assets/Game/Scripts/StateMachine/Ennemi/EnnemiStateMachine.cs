using UnityEngine;
using EnnemiState;
using EnnemiTransition;
using NaughtyAttributes;
using UnityEngine.AI;
using Deps = DependenciesInjector;

public class EnnemiStateMachine : IStateMachine
{
    [Header("Configuration")]
    [SerializeField] Transform _parent;
    [SerializeField] Animator _animator;
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] LifeScript _life;
    [SerializeField] FollowTransform _followTransform;
    [SerializeField] WeaponScript _weapon;
    [SerializeField] float _counterTime;

    [Button]
    void TryGetConfiguration()
    {
        if(_agent == null)
            _agent = _parent.gameObject.GetComponent<NavMeshAgent>();
        if(_life == null)
            _life = _parent.gameObject.GetComponent<LifeScript>();
        if(_weapon == null)
            _weapon = _parent.gameObject.GetComponentInChildren<WeaponScript>();
    }
    
    Parade _playerParry;
    
    protected override void Start()
    {
        _playerParry = Deps.PDeps.Parry;
        
        _life.OnDamaged += () =>
        {
            _weapon.CancelAttack();
            _animator.SetTrigger("Hit");
        };
        UpdateSpeedAnim();
        
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
        
        //Define first state
        base.Start();
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
