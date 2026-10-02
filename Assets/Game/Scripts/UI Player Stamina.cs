using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStamina : MonoBehaviour
{
    [SerializeField] StaminaHandler _player;
    [SerializeField] Slider _slider;
    
    void Reset()
    {
        _slider = GetComponent<Slider>();
        _player = GameObject.FindObjectOfType<StaminaHandler>();
    }
    
    void Start()
    {
        _slider.maxValue = _player.MaxStamina;
        _player.OnDrain += UpdateSlider;
        _player.OnRegen += UpdateSlider;
        UpdateSlider();
    }

    void OnDestroy()
    {
        _player.OnDrain -= UpdateSlider;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    void UpdateSlider()
    {
        _slider.value = _player.Stamina;
    }
}
