using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Parade : MonoBehaviour
{
    [Header("Stats")]
    //[SerializeField] float _recoveryTime;
    [SerializeField] float _recoveryTime;
    [SerializeField] float _parryTime;
    
    public event Action OnParryStart;
    public event Action OnParryEnd;
    
    bool _isRecovering;
    
    [Header("Dependencies")]
    [SerializeField] LivingEntity _entity;

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

    public async void StopAttack()
    {
        if (IsReady == false)
            return;
        // Ready !

        _entity.IsInvincible = true;
        OnParryStart?.Invoke();
        Debug.Log("Invincible");
        
        await Awaitable.WaitForSecondsAsync(_parryTime, destroyCancellationToken);
        if (destroyCancellationToken.IsCancellationRequested)
            return;
        
        _entity.IsInvincible = false;
        OnParryEnd?.Invoke();
        Debug.Log("Not invincible");

        // Not ready
        _isRecovering = true;
        await Awaitable.WaitForSecondsAsync(_recoveryTime, destroyCancellationToken);
        if (destroyCancellationToken.IsCancellationRequested)
            return;

        _isRecovering = false;
        Debug.Log("Parry ready");
    }
}
