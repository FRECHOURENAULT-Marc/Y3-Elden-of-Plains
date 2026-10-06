using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    [SerializeField] LifeScript _life;
    [SerializeField] Slider _slider;

    void Reset()
    {
        _slider = GetComponent<Slider>();
    }

    public void Init(LifeScript lifeScript)
    {
        _life = lifeScript;
    }
    void Start()
    {
        _slider.maxValue = _life.MaxLife;
        _life.OnDamaged += UpdateSlider;
        UpdateSlider();
    }

    void OnDestroy()
    {
        _life.OnDamaged -= UpdateSlider;
    }
    
    void UpdateSlider()
    {
        _slider.value = _life.Life;
    }
}
