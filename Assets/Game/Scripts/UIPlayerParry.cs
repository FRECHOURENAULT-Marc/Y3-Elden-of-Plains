using UnityEngine;
using UnityEngine.UI;

public class UIPlayerParry : MonoBehaviour
{
    [SerializeField] Parade _parry;
    [SerializeField] Image _uiImage;

    void Reset()
    {
        _uiImage = GetComponent<Image>();
    }
    
    void Start()
    {
        _uiImage.enabled = false;
        
        _parry.OnParryStart += () => _uiImage.enabled = true;
        _parry.OnParryEnd += () => _uiImage.enabled = false;
    }
    
}
