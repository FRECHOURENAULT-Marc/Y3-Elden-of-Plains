using UnityEngine;

public class UIEnnemiLifeInitializer : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] GameObject _prefab;
    [SerializeField] LivingEntity _lifeScript;
    
    RectTransform _uiParent;
    
    void Start()
    {
        _uiParent = transform.GetChild(0).GetComponent<RectTransform>();
        
        GameObject go = GameObject.Instantiate(_prefab, _uiParent);
        UIEnnemiLife uiScaler = go.GetComponent<UIEnnemiLife>();
        uiScaler.WorldAnchor = transform;
        UIHealth ui = go.GetComponent<UIHealth>();
        ui.Init(_lifeScript);
        
        _lifeScript.OnDeath += () => Destroy(go);
    }
}
