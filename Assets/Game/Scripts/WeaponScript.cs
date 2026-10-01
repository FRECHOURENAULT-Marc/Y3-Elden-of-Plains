using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WeaponScript : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] ColliderEvent _collider;
    [SerializeField] LifeScript.GO_TEAM _team;
    
    [Header("Stats")]
    [SerializeField] float _damage;
    [SerializeField] float _beforeDamageTime;
    [SerializeField] float _attackTime;
    [SerializeField] float _recoveryTime;
    
    [Header("Events")]
    [SerializeField] UnityEvent _onStartCharging;
    [SerializeField] UnityEvent _onStartAttacking;
    [SerializeField] UnityEvent _onContact;

    float _beforeDamageTimer;
    float _recoveryTimer;
    float _attackTimer;
    List<LifeScript> _damagedEntities;

    public bool IsInAttackingState
    {
        get => _attackTimer > 0;
    }      
    public bool IsAttackReady
    {
        get => _recoveryTimer > _recoveryTime && (IsCurrentlyAttacking == false);
    }    
    public bool IsCurrentlyAttacking
    {
        get => (_beforeDamageTimer > 0 && _beforeDamageTimer < _beforeDamageTime) 
               || (_attackTimer > 0 && _attackTimer < _attackTime);
    }

    void Reset()
    {
        _damage = 1.0f;
        _beforeDamageTime = 0.25f;
        _attackTime = 1.0f;
        _recoveryTime = 1.0f;
    }
    
    void Start()
    {
        // Test if dependencies are filled
        if(_collider == null)
        {
            Debug.LogError("No collider defined");
            Destroy(this);
            return;
        }
        
        _damagedEntities = new List<LifeScript>();
        _collider.onTriggerStay += InflictDamage;
        _recoveryTimer = _recoveryTime;
    }

    void ResetFields()
    {
        _damagedEntities.Clear();
        _beforeDamageTimer = 0;
        _attackTimer = 0;
        _recoveryTimer = 0;
    }

    void InflictDamage(Collider other)
    {
        // Do not apply damage if _timerBeforeDamage is not complete
        if (IsInAttackingState == false)
            return;
        
        // Be sure the target is a living entity
        if (other.TryGetComponent(out LifeProxy proxy) == false)
            return;
        LifeScript life = proxy._script;
        // Be sure the target is not damaged more than one time by the same attack
        if (_damagedEntities.Contains(life))
            return;
        // Be sure the target is not in the same team as the weapon
        if (life.GoTeam == _team)
            return;
        
        _onContact?.Invoke();
        life.TryDamaging(_damage);
        _damagedEntities.Add(life);
    }

    public void Attack() => AttackAsync();

    public async void AttackAsync(CancellationToken cancel = default)
    {
        if(cancel == CancellationToken.None)
            cancel = destroyCancellationToken;

        if (IsAttackReady == false)
            return;
        
        // Ready !
        
        // Time before attacking
        _onStartCharging?.Invoke();
        float startTime = Time.time;
        while (_beforeDamageTimer < _beforeDamageTime)
        {
            await Awaitable.NextFrameAsync(cancel);
            if (cancel.IsCancellationRequested)
                return;
            
            _beforeDamageTimer = Time.time - startTime;
        }
        
        // Attack time
        _onStartAttacking?.Invoke();
        startTime = Time.time;
        while (_attackTimer < _attackTime)
        {
            await Awaitable.NextFrameAsync(cancel);
            if (cancel.IsCancellationRequested)
                return;
            
            _attackTimer = Time.time - startTime;
        }        
        
        // Reset fields (timers & _damagedEntities)
        ResetFields();
    }

    void FixedUpdate()
    {
        if (IsCurrentlyAttacking)
            return;

        if (_recoveryTimer > _recoveryTime)
            return;
        
        _recoveryTimer += Time.fixedDeltaTime;
    }
}
