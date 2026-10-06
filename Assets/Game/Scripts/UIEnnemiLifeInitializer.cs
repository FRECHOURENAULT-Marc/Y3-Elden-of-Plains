using UnityEngine;

public class UIEnnemiLifeInitializer : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] GameObject _prefab;
    [SerializeField] LifeScript _lifeScript;
    
    RectTransform _uiParent;
    
    void Start()
    {
        _uiParent = DependenciesInjector.UIDeps.EnnemiHealthParent;
        
        GameObject go = GameObject.Instantiate(_prefab, _uiParent);
        UIEnnemiLife uiScaler = go.GetComponent<UIEnnemiLife>();
        UIHealth ui = go.GetComponent<UIHealth>();
        uiScaler.Init(transform);
        ui.Init(_lifeScript);
    }
}
