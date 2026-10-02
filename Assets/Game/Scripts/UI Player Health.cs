using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealth : MonoBehaviour
{
    [SerializeField] LifeScript _playerLife;
    [SerializeField] Slider _slider;

    void Reset()
    {
        _slider = GetComponent<Slider>();
    }
    
    void Start()
    {
        _slider.maxValue = _playerLife.MaxLife;
        _playerLife.OnDamaged += UpdateSlider;
        UpdateSlider();
    }

    void OnDestroy()
    {
        _playerLife.OnDamaged -= UpdateSlider;
    }
    
    void UpdateSlider()
    {
        _slider.value = _playerLife.Life;
    }
}
