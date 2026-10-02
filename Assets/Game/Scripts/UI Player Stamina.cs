using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStamina : MonoBehaviour
{
    [SerializeField] StaminaHandler _playerStamina;
    [SerializeField] Slider _slider;
    
    void Reset()
    {
        _slider = GetComponent<Slider>();
    }
    
    void Start()
    {
        _slider.maxValue = _playerStamina.MaxStamina;
        _playerStamina.OnDrain += UpdateSlider;
        _playerStamina.OnRegen += UpdateSlider;
        UpdateSlider();
    }

    void OnDestroy()
    {
        _playerStamina.OnDrain -= UpdateSlider;
        _playerStamina.OnRegen -= UpdateSlider;
    }
    
    void UpdateSlider()
    {
        _slider.value = _playerStamina.Stamina;
    }
}
