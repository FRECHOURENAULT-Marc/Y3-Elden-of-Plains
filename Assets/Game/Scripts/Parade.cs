using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Parade : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float _recoveryTime;
    [SerializeField] float _parryTime;
    
    float _recoveryTimer;
    float _parryTimer;
    
    [Header("Dependencies")]
    [SerializeField] LivingEntity _entity;

    public bool IsParring
    {
        get { return _parryTimer > 0; }
    }
    
    void Reset()
    {
        _recoveryTime = 1.0f;
        _parryTime = 1.0f;
    }

    public async void StopAttack()
    {
        if (_recoveryTimer < _recoveryTime)
            return;
        // Ready !
        _recoveryTimer = 0;

        _entity.IsInvincible = true;
        Debug.Log("Invincible");
        
        float startTime = Time.time;
        while (_parryTimer < _parryTime)
        {
            await Awaitable.NextFrameAsync();
            _parryTimer = Time.time - startTime;

        }
        
        _entity.IsInvincible = false;
        _parryTimer = 0;
        Debug.Log("Not invincible");
    }
    
    void Update()
    {
        if (IsParring)
            return;
        _recoveryTimer += Time.deltaTime;
    }
}
