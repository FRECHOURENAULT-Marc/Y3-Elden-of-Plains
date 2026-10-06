using System;
using UnityEngine;
using Deps = DependenciesInjector;

public class Parade : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float _recoveryTime;
    [SerializeField] float _parryTime;
    
    public event Action OnParryStart;
    public event Action OnParryEnd;
    
    LivingEntity _entity;
    bool _isRecovering;

    public float ParryTime { get => _parryTime;}
    public float RecoveryTime { get => _recoveryTime;}
    
    public bool IsParring
    {
        get => _entity.IsInvincible; 
    }    
    public bool IsReady
    {
        get => IsParring == false && _isRecovering == false; 
    }
    
    void Reset()
    {
        _recoveryTime = 1.0f;
        _parryTime = 1.0f;
    }

    void Start()
    {
        _entity = Deps.PDeps.Life;
    }

    public async void StopAttack()
    {
        if (IsReady == false)
            return;
        // Ready !

        _entity.IsInvincible = true;
        OnParryStart?.Invoke();
        
        await Awaitable.WaitForSecondsAsync(_parryTime, destroyCancellationToken);
        if (destroyCancellationToken.IsCancellationRequested)
            return;
        
        _entity.IsInvincible = false;
        OnParryEnd?.Invoke();

        // Not ready
        _isRecovering = true;
        await Awaitable.WaitForSecondsAsync(_recoveryTime, destroyCancellationToken);
        if (destroyCancellationToken.IsCancellationRequested)
            return;

        _isRecovering = false;
    }
}
