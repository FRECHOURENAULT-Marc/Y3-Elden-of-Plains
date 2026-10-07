using UnityEngine;
using UnityEngine.Animations;

public class UIEnnemiLifeInitializer : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] GameObject _prefab;
    [SerializeField] LivingEntity _lifeScript;
    
    RectTransform _uiParent;
    
    void Start()
    {
        _uiParent = transform.GetChild(0).GetComponent<RectTransform>();
        
        GameObject go = Instantiate(_prefab, _uiParent);
        UIEnnemiLife uiScaler = go.GetComponent<UIEnnemiLife>();
        uiScaler.WorldAnchor = transform;
        UIHealth ui = go.GetComponent<UIHealth>();
        ui.Init(_lifeScript);
        
        ConstraintSource cs = new ConstraintSource
        {
            sourceTransform = Camera.main.transform,
            weight = 1
        };
        transform.GetChild(0).GetComponent<LookAtConstraint>().AddSource(cs);
    }
}
