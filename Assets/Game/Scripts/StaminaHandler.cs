using System;
using System.Threading;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class StaminaHandler : MonoBehaviour
{
    [SerializeField] float _maxStamina;
    [SerializeField] float _staminaRegenRate;
    [SerializeField] float _startRegenTime;
    [SerializeField] float _exhaustTime;
     
    [ShowNonSerializedField] float _stamina;
    float _startRegenTimer;
    float _exhaustTimer;
    CancellationTokenSource _cts;
    
    public event Action OnDrain;
    public event Action OnRegen;

    public bool IsExhaust
    {
        get => _exhaustTimer > 0;
    }    
    bool IsDraining
    {
        get => _startRegenTimer > 0;
    }
    
    public bool HasStamina
    {
        get => _stamina > 0;
    }

    public float Stamina
    {
        get => _stamina;
    }
    
    public float MaxStamina
    {
        get => _maxStamina;
    }
    
    void Reset()
    {
        _maxStamina = 100;
        _staminaRegenRate = 10;
        _startRegenTime = 5;
        _exhaustTime = 10;
    }
    void Awake()
    {
        _stamina = _maxStamina;
        _cts = new CancellationTokenSource();
    }

    async void HandleExhaust()
    {
        _exhaustTimer = 0.0001f;
        await Awaitable.WaitForSecondsAsync(_exhaustTime, destroyCancellationToken);
        if (destroyCancellationToken.IsCancellationRequested)
            return;
        _exhaustTimer = 0;
    }    
    async void HandleStartRegen(CancellationToken cancel)
    {
        _startRegenTimer = 0.0001f;
        await Awaitable.WaitForSecondsAsync(_startRegenTime, cancel);
        if (cancel.IsCancellationRequested)
            return;
        _startRegenTimer = 0;
    }

    public bool HasEnough(float amount)
    {
        return (_stamina - amount) > 0;
    }
    
    public void DrainStamina(float amount)
    {
        if (HasStamina == false)
            return;
        
        _stamina -= amount;
        OnDrain?.Invoke();
        
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        _cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, _cts.Token);
        HandleStartRegen(_cts.Token);

        if (HasStamina)
            return;

        _stamina = 0;
        HandleExhaust();
    }

    void Update()
    {
        if (IsExhaust || IsDraining)
            return;
        if (_stamina == _maxStamina)
            return;
        
        OnRegen?.Invoke();
        _stamina += _staminaRegenRate*Time.deltaTime;
        if (_stamina > _maxStamina)
            _stamina = _maxStamina;
    }
}
