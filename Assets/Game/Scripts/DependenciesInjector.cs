using System;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public abstract class Dependencies
{
    [SerializeField] protected GameObject _mainParent;
    
    public void TryGetComponent<C>(GameObject go, ref C component) where C : Component
    {
        if (component != null) return;
        
        go.TryGetComponent(out component);
        if (component == null)
            component = go.GetComponentInChildren<C>();
    }
    
    public void TryGettingFieldsFromParent() => TryGettingFields(_mainParent);

    public abstract void TryGettingFields(GameObject gameObject);
}

[Serializable] public class PlayerDependencies : Dependencies
{
    // Fields
    [SerializeField] Rigidbody _rb;
    [SerializeField] LivingEntity _life;
    [SerializeField] WeaponScript _weapon;
    [SerializeField] Animator _animator;
    [SerializeField] Collider _collider;
    [SerializeField] GameObject _pActions;
    [SerializeField] Parade _parry;
    [SerializeField] StaminaHandler _stamina;
    
    // Properties
    public GameObject Player { get => _mainParent; } // "rename"
    public Rigidbody Rb { get => _rb; }
    public LivingEntity Life { get => _life; }
    public WeaponScript Weapon { get => _weapon; }
    public Animator Animator { get => _animator; }
    public GameObject PActions { get => _pActions; }
    public Collider Collider { get => _collider; }
    public Parade Parry { get => _parry; }
    public StaminaHandler Stamina { get => _stamina; }
    
    public override void TryGettingFields(GameObject gameObject)
    {
        TryGetComponent(gameObject, ref _rb);
        TryGetComponent(gameObject, ref _life);
        TryGetComponent(gameObject, ref _weapon);
        // Animator has special place
        TryGetComponent(gameObject, ref _collider);

        // PActions
        TryGetComponent(_pActions, ref _parry);
        TryGetComponent(_pActions, ref _stamina);
    }
}

[Serializable] public class RenderDependencies : Dependencies
{
    // Fields
    [SerializeField] Camera _mainCamera;
    [SerializeField] GameObject _cinemachine;
    [SerializeField] CinemachineCamera _cinemachineCamera;
    
    // Properties
    public GameObject Camera { get => _mainParent; } // "rename"
    public Camera MainCamera { get => _mainCamera; }
    public GameObject Cinemachine { get => _cinemachine; }
    public CinemachineCamera CinemachineCamera { get => _cinemachineCamera; }

    
    public override void TryGettingFields(GameObject gameObject)
    {
        TryGetComponent(gameObject, ref _mainCamera);
        
        // Cinemachine
        TryGetComponent(_cinemachine, ref _cinemachineCamera);
    }
}

[Serializable] public class UIDependencies : Dependencies
{
    // Fields
    [SerializeField] GameObject _playerParent;
    [SerializeField] GameObject _ennemiParent;
    [SerializeField] RectTransform _ennemiHealthParent;
    
    // Properties
    public GameObject Canvas { get => _mainParent; } // "rename"
    public GameObject EnnemiParent { get => _ennemiParent; }
    public GameObject Player { get => _playerParent; }
    public RectTransform EnnemiHealthParent { get => _ennemiHealthParent; }

    
    public override void TryGettingFields(GameObject gameObject)
    {
        if(_playerParent == null)
            _playerParent = gameObject.transform.GetChild(0).gameObject;
        if(_ennemiParent == null)
            _ennemiParent = gameObject.transform.GetChild(1).gameObject;
        
        TryGetComponent(_ennemiParent.transform.GetChild(0).gameObject, ref _ennemiHealthParent);
    }
}

public class DependenciesInjector : MonoBehaviour
{
    [SerializeField] PlayerDependencies _playerDeps;
    public static PlayerDependencies PDeps;    
    [Button("Get Player Dependencies")]
    void TryGettingFieldsP() => _playerDeps.TryGettingFieldsFromParent();
    
    [SerializeField] RenderDependencies _renderDeps;
    public static RenderDependencies RDeps;
    [Button("Get Render Dependencies")]
    void TryGettingFieldsR() => _renderDeps.TryGettingFieldsFromParent();
        
    [SerializeField] UIDependencies _uiDeps;
    public static UIDependencies UIDeps;
    [Button("Get UI Dependencies")]
    void TryGettingFieldsUI() => _uiDeps.TryGettingFieldsFromParent();
    
    private void Awake()
    {
        PDeps = _playerDeps;        
        RDeps = _renderDeps;
        UIDeps = _uiDeps;
    }
}
