using UnityEngine;
using UnityEngine.UI;
using Deps = DependenciesInjector;

public class UIPlayerStamina : MonoBehaviour
{
    [SerializeField] Slider _slider;
    
    StaminaHandler _playerStamina;
    
    void Reset()
    {
        _slider = GetComponent<Slider>();
    }
    
    void Start()
    {
        _playerStamina = Deps.PDeps.Stamina;
        
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
