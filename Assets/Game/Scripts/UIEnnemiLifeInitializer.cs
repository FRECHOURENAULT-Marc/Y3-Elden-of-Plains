using UnityEngine;

public class UIEnnemiLifeInitializer : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] GameObject _prefab;
    [Header("Dependencies")]
    [SerializeField] LifeScript _lifeScript;
    [SerializeField] RectTransform _uiParent;
    
    void Start()
    {
        GameObject go = GameObject.Instantiate(_prefab, _uiParent);
        UIEnnemiLife uiScaler = go.GetComponent<UIEnnemiLife>();
        UIHealth ui = go.GetComponent<UIHealth>();
        uiScaler.Init(transform);
        ui.Init(_lifeScript);
    }
}
