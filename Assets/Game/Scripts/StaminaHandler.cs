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
    
    [Header("Debug")]
    [SerializeField] TextMeshProUGUI _debugText;
     
    [ShowNonSerializedField] float _stamina;
    float _startRegenTimer;
    float _exhaustTimer;
    CancellationTokenSource _cts;

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
    
    void Reset()
    {
        _maxStamina = 100;
        _staminaRegenRate = 10;
        _startRegenTime = 5;
        _exhaustTime = 10;
    }
    void Start()
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
        
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        _cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, _cts.Token);
        HandleStartRegen(_cts.Token);
        
        //Debug
        Debug.Log($"Stamina {amount} has been drained.");

        if (HasStamina)
            return;

        _stamina = 0;
        HandleExhaust();
    }

    void Update()
    {
        _debugText.SetText($"Stamina {Mathf.RoundToInt(_stamina)}");
        
        if (IsExhaust || IsDraining)
            return;

        _stamina += _staminaRegenRate*Time.deltaTime;
        if (_stamina > _maxStamina)
            _stamina = _maxStamina;
    }
}
