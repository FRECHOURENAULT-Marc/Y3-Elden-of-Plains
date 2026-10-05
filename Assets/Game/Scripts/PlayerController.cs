using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = System.Object;

public class PlayerController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] LifeScript _life;
    [SerializeField] StaminaHandler _stamina;
    [SerializeField] Parade _parry;
    [SerializeField] WeaponScript _weapon;
    
    [SerializeField] Transform _parent;
    [SerializeField] Rigidbody _rb;
    [SerializeField] CinemachineCamera _camera;
    [SerializeField] Animator _animator;
    
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
    
    CinemachineOrbitalFollow _cameraOrbital;
    bool _isGrounded;
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
        _cameraOrbital = _camera.GetComponent<CinemachineOrbitalFollow>();
        
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
        
        _jumpInput.action.started -= Jump;
        
        _attackInput.action.started -= Attack;

        _parryInput.action.started -= Parry;

        _life.OnDamaged -= OnHit;
    }

    void MoveUpdate(InputAction.CallbackContext obj)
    {
        _joystickDirection = obj.ReadValue<Vector2>();
        _joystickDirection = new Vector3(_joystickDirection.x, 0f, _joystickDirection.y);
    }
    void MoveStop(InputAction.CallbackContext obj) => _joystickDirection = Vector3.zero;
    void Jump(InputAction.CallbackContext obj)
    {
        if (_weapon.IsCurrentlyAttacking)
            return;
        if (_parry.IsParring)
            return;
        if (_isGrounded == false)
            return;
        if (_stamina.HasStamina == false)
            return;
        
        _rb.AddForce(Vector3.up * _jumpForce);
        _stamina.DrainStamina(_jumpEnergy);
        _animator.SetTrigger("Jump");
    }
    void Attack(InputAction.CallbackContext obj)
    {
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
        if (_isGrounded == false)
            return;
        if (_weapon.IsAttackReady == false)
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
        if (_weapon.IsCurrentlyAttacking)
            return;        
        if (_parry.IsParring)
            return;
        
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

    bool ColliderHasGroundTag(Collision other) 
        => other.gameObject.TryGetComponent(out GroundTag gt);
  
    bool ColliderHasGroundTag(Collider other) 
        => other.gameObject.TryGetComponent(out GroundTag gt);
    
    void OnCollisionEnter(Collision other)
    {
        if (ColliderHasGroundTag(other) == false)
            return;

        _isGrounded = true;
    }    
    void OnCollisionExit(Collision other)
    {
        if (ColliderHasGroundTag(other) == false)
            return;
        
        _isGrounded = false;
    }    
    
    void OnTriggerEnter(Collider collider)
    {
        if (ColliderHasGroundTag(collider) == false)
            return;

        _isGrounded = true;
    }    
    void OnTriggerExit(Collider collider)
    {
        if (ColliderHasGroundTag(collider) == false)
            return;
        
        _isGrounded = false;
    }


}
