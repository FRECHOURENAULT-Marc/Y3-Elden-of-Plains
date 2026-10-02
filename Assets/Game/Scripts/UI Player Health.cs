using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealth : MonoBehaviour
{
    [SerializeField] LifeScript _player;
    [SerializeField] Slider _slider;

    void Reset()
    {
        _slider = GetComponent<Slider>();
        _player = GameObject.FindObjectOfType<LifeScript>();
    }
    
    void Start()
    {
        _slider.maxValue = _player.MaxLife;
        _player.OnDamaged += UpdateSlider;
        UpdateSlider();
    }

    // On pense à se désinscrire !
    void OnDestroy()
    {
        _player.OnDamaged -= UpdateSlider;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    void UpdateSlider()
    {
        _slider.value = _player.Life;
    }
}
