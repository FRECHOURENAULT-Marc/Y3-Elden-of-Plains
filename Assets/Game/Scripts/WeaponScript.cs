using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Deps = DependenciesInjector;

public class WeaponScript : MonoBehaviour
{
    public enum ATTACK_STATE
    {
        Ready,
        Charging,
        Attacking,
        Recovery
    }
    
    [Header("Configuration")]
    [SerializeField] LifeScript.GO_TEAM _team;
    
    [Header("Stats")]
    [SerializeField] float _damage;
    [SerializeField] float _beforeDamageTime;
    [SerializeField] float _attackTime;
    [SerializeField] float _recoveryTime;
    [SerializeField] float _weight;
    
    [Header("Events")]
    [SerializeField] UnityEvent _onStartCharging;
    [SerializeField] UnityEvent _onStartAttacking;
    [SerializeField] UnityEvent _onContact;
    
    List<LifeScript> _damagedEntities;
    
    private ATTACK_STATE _attackState = ATTACK_STATE.Ready;
    private bool _attackCanceled = false;

    public float Weight
    {
        get => _weight;
    }
    
    public bool IsCurrentlyAttacking =>
        _attackState == ATTACK_STATE.Charging ||
        _attackState == ATTACK_STATE.Attacking;

    public bool IsAttackReady =>
        _attackState == ATTACK_STATE.Ready;

    public UnityEvent OnContact { get => _onContact; }
    public UnityEvent OnStartCharging { get => _onStartCharging; }
    public UnityEvent OnStartAttacking { get => _onStartAttacking; }
    
    void Reset()
    {
        _damage = 1.0f;
        _beforeDamageTime = 0.25f;
        _attackTime = 1.0f;
        _recoveryTime = 1.0f;
        _weight = 20.0f;
    }
    
    void Start()
    {
        _damagedEntities = new List<LifeScript>();
    }

    public void CancelAttack() => _attackCanceled = true;
    
    void TryInflictDamageToCollider(Collider other)
    {
        if (_attackState != ATTACK_STATE.Attacking)
            return;
        if(_attackCanceled)
            return;
        
        if (other.TryGetComponent(out LifeProxy proxy) == false)
            return;
        LifeScript life = proxy._script;
        
        if (_damagedEntities.Contains(life))
            return;

        if (life.GoTeam == _team)
            return;

        _onContact?.Invoke();
        life.TryDamaging(_damage);

        _damagedEntities.Add(life);
    }

    public void Attack() => AttackAsync();

    private async Awaitable AttackAsync(CancellationToken cancel = default)
    {
        if (IsAttackReady == false)
            return;

        if (cancel == CancellationToken.None)
            cancel = destroyCancellationToken;

        // Charging
        _attackState = ATTACK_STATE.Charging;
        _onStartCharging?.Invoke();

        await Awaitable.WaitForSecondsAsync(_beforeDamageTime, cancel);

        if (cancel.IsCancellationRequested)
            return;

        // Attack
        _attackState = ATTACK_STATE.Attacking;
        _onStartAttacking?.Invoke();

        await Awaitable.WaitForSecondsAsync(_attackTime, cancel);

        if (cancel.IsCancellationRequested)
            return;

        // Recovery
        _attackState = ATTACK_STATE.Recovery;

        await Awaitable.WaitForSecondsAsync(_recoveryTime, cancel);

        if (cancel.IsCancellationRequested)
            return;

        _attackState = ATTACK_STATE.Ready;
        _damagedEntities.Clear();
        _attackCanceled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        TryInflictDamageToCollider(other);
    }    
    void OnCollisionEnter(Collision other)
    {
        TryInflictDamageToCollider(other.collider);
    }
}
