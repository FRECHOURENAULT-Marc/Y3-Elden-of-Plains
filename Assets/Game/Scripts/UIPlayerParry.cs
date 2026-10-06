using UnityEngine;
using UnityEngine.UI;

public class UIPlayerParry : MonoBehaviour
{
    [SerializeField] Image _uiImage;

    Parade _parry;
    
    void Reset()
    {
        _uiImage = GetComponent<Image>();
    }
    
    void Start()
    {
        _parry = DependenciesInjector.PDeps.Parry;
        
        _uiImage.enabled = false;
        
        _parry.OnParryStart += () => _uiImage.enabled = true;
        _parry.OnParryEnd += () => _uiImage.enabled = false;
    }
    
}
