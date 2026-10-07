using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

[Serializable]

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
    [SerializeField] ParticuleData _hitPart;
    
    [Header("Stats")]
    [SerializeField] float _damage;
    [SerializeField] float _beforeDamageTime;
    [SerializeField] float _attackTime;
    [SerializeField] float _recoveryTime;
    [SerializeField] float _weight;
    
    public event Action<Collider> OnContact;
    public event Action<Collider> OnDamageApplied;
    ParticulePlayer _particulePlayer;
    List<LifeScript> _damagedEntities;
    ATTACK_STATE _attackState = ATTACK_STATE.Ready;
    bool _attackCanceled = false;

    public float Weight
    {
        get => _weight;
    }
    
    public bool IsCurrentlyAttacking =>
        _attackState == ATTACK_STATE.Charging ||
        _attackState == ATTACK_STATE.Attacking;

    public bool IsAttackReady =>
        _attackState == ATTACK_STATE.Ready;
    
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
        _particulePlayer = GetComponent<ParticulePlayer>();
        _damagedEntities = new List<LifeScript>();
        OnDamageApplied += SpawnHitParticules;
    }

    public void CancelAttack() => _attackCanceled = true;

    void SpawnHitParticules(Collider other)
    {
        Vector3 point = other.ClosestPoint(transform.position);
        _particulePlayer.PlayEffect(_hitPart.name, point, _hitPart.scale);
    }
    
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

        OnContact?.Invoke(other);
        if (life.TryDamaging(_damage))
            OnDamageApplied?.Invoke(other);
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
        //_onStartCharging?.Invoke();

        await Awaitable.WaitForSecondsAsync(_beforeDamageTime, cancel);
        if (cancel.IsCancellationRequested)
            return;

        // Attack
        _attackState = ATTACK_STATE.Attacking;
        //_onStartAttacking?.Invoke();

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

    void OnTriggerStay(Collider other)
    {
        TryInflictDamageToCollider(other);
    }    
    void OnCollisionStay(Collision other)
    {
        TryInflictDamageToCollider(other.collider);
    }
}
