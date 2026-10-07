using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Deps = DependenciesInjector;

public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] InputActionReference _moveInput;
    [SerializeField] InputActionReference _LookInput;
    [SerializeField] InputActionReference _jumpInput;
    [SerializeField] InputActionReference _attackInput;
    [SerializeField] InputActionReference _parryInput;
    [Header("Configuration")]
    [SerializeField] float _speed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpEnergy;
    
    
    LifeScript _life;
    GroundChecker _ground;
    StaminaHandler _stamina;
    Parade _parry;
    WeaponScript _weapon;
    
    Transform _parent;
    Rigidbody _rb;
    Animator _animator;
    CinemachineCamera _camera;
    CinemachineOrbitalFollow _cameraOrbital;
    
    bool _isFreeLook;
    Vector3 _joystickDirection;
    float _oldVerticalAxis;
    float _oldHorizontalAxis;
    Vector3 _oldRight;
    Vector3 _oldForward;
    
    void Reset()
    {
        _rb = GetComponent<Rigidbody>();
        
        _speed = 10f;
        _jumpForce = 1000f;
        _jumpEnergy = 20f;
    }
    
    void Start()
    {
        _life = Deps.PDeps.Life;
        _ground = Deps.PDeps.GroundChecker;
        _stamina = Deps.PDeps.Stamina;
        _parry = Deps.PDeps.Parry;
        _parent = Deps.PDeps.Player.transform;
        _rb = Deps.PDeps.Rb;
        _animator = Deps.PDeps.Animator;
        
        _camera = Deps.RDeps.CinemachineCamera;
        
        _cameraOrbital = _camera.GetComponent<CinemachineOrbitalFollow>();
        
        Deps.PDeps.ItemChecker.OnTake += OnTake;
        
        _moveInput.action.started += MoveUpdate;
        _moveInput.action.performed += MoveUpdate;
        _moveInput.action.canceled += MoveStop;

        _LookInput.action.started += ActivateFreeLook;
        _LookInput.action.canceled += DisableFreeLook;
        
        _jumpInput.action.started += Jump;
        
        _attackInput.action.started += Attack;

        _parryInput.action.started += Parry;

        _life.OnDamaged += OnHit;
    }

    void OnDestroy()
    {
        _moveInput.action.started -= MoveUpdate;
        _moveInput.action.performed -= MoveUpdate;
        _moveInput.action.canceled -= MoveStop;
        
        _LookInput.action.started -= ActivateFreeLook;
        _LookInput.action.canceled -= DisableFreeLook;
        
        _jumpInput.action.started -= Jump;
        
        _attackInput.action.started -= Attack;

        _parryInput.action.started -= Parry;

        _life.OnDamaged -= OnHit;
    }

    void TakeWeapon(WeaponScript weapon)
    {
        _weapon = weapon;
        Rigidbody rb = _weapon.GetComponent<Rigidbody>();
        if (rb.isKinematic == false)
            rb.isKinematic = true;
    } 
    void OnTake(ItemScript old, ItemScript n)
    {
        if (n.TryGetComponent(out WeaponScript weapon))
        {
            TakeWeapon(weapon);
            return;
        }
    }
    
    void MoveUpdate(InputAction.CallbackContext obj)
    {
        _joystickDirection = obj.ReadValue<Vector2>();
        _joystickDirection = new Vector3(_joystickDirection.x, 0f, _joystickDirection.y);
    }
    void MoveStop(InputAction.CallbackContext obj) => _joystickDirection = Vector3.zero;
    void Jump(InputAction.CallbackContext obj)
    {
        if (_weapon)
        {
            if (_weapon.IsCurrentlyAttacking)
                return;
            if (_parry.IsParring)
                return;
        }

        if (_ground.IsGrounded == false)
            return;
        if (_stamina.HasStamina == false)
            return;
        
        _rb.AddForce(Vector3.up * _jumpForce);
        _stamina.DrainStamina(_jumpEnergy);
        _animator.SetTrigger("Jump");
    }
    void Attack(InputAction.CallbackContext obj)
    {
        if (_weapon == null)
            return;
        if (_weapon.IsAttackReady == false)
            return;
        if (_stamina.HasStamina == false)
            return;
        
        _weapon.Attack();
        _stamina.DrainStamina(_weapon.Weight);
        _animator.SetTrigger("Attack");
    }
    void Parry(InputAction.CallbackContext obj)
    {
        if (_weapon == null)
            return;
        if (_ground.IsGrounded == false)
            return;
        if (_weapon.IsCurrentlyAttacking)
            return;
        if (_parry.IsReady == false)
            return;
        
        _parry.StopAttack();
        _animator.SetTrigger("Parry");
    }
    void ActivateFreeLook(InputAction.CallbackContext obj)
    {
        _isFreeLook = true;
        _oldHorizontalAxis = _cameraOrbital.HorizontalAxis.Value;
        _oldVerticalAxis = _cameraOrbital.VerticalAxis.Value;

    }
    void DisableFreeLook(InputAction.CallbackContext obj)
    {
        _isFreeLook = false;
        _cameraOrbital.HorizontalAxis.Value = _oldHorizontalAxis;
        _cameraOrbital.VerticalAxis.Value = _oldVerticalAxis;
    } 
    
    void OnHit() => _animator.SetTrigger("Hit");
    
    void Update()
    {
        if (_weapon)
        {
            if (_weapon.IsCurrentlyAttacking)
                return;        
            if (_parry.IsParring)
                return;
        }
        
        var directionFinale = _joystickDirection;
        // On récupère les axes de la cam pour les retravailler et les exploiter dans notre direction
        if (_isFreeLook == false)
        {
            _oldRight = _camera.transform.right;
            _oldForward = _camera.transform.forward;
        }
        Vector3 f = _oldForward;
        f.y = 0f;
        var r = _oldRight;
            
        // Calcul de la direction finale
        var dir = Vector3.zero;
        dir = (f * directionFinale.z) + (r * directionFinale.x);
        
        float staminaFactor = _stamina.IsExhaust ? 0.5f : 1;
        var translation = dir * (_speed * Time.deltaTime * staminaFactor);
        _parent.Translate(translation, Space.World);
        _animator.SetFloat("Speed", translation.normalized.magnitude * _speed);

        // Rotation du personnage
        if (_isFreeLook)
        {
            if (dir == Vector3.zero)
                return;
            _parent.forward = dir;
        }
        else
        {
            Vector3 ff = _camera.transform.forward;
            ff.y = 0f;
            _parent.forward = ff;
        }
            
    }

}
