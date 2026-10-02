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
    [SerializeField] Camera _camera;
    [SerializeField] Animator _animator;
    
    [Header("Inputs")]
    [SerializeField] InputActionReference _moveInput;
    [SerializeField] InputActionReference _jumpInput;
    [SerializeField] InputActionReference _attackInput;
    [SerializeField] InputActionReference _parryInput;
    [Header("Configuration")]
    [SerializeField] float _speed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpEnergy;
    /*[SerializeField]*/ float _mouseSensitivity;

    
    bool _isGrounded;
    Vector3 _joystickDirection;
    private float _rotationY;
    
    void Reset()
    {
        _rb = GetComponent<Rigidbody>();
        
        _speed = 10f;
        _jumpForce = 1000f;
        _jumpEnergy = 20f;
    }
    
    void Start()
    {
        _moveInput.action.started += MoveUpdate;
        _moveInput.action.performed += MoveUpdate;
        _moveInput.action.canceled += MoveStop;
        
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

    void OnHit()
    {
        _animator.SetTrigger("Hit");
    }

    void HandleRotation() // En attente
    {
        float mouseX = Input.GetAxis("Mouse X");
        _rotationY += mouseX * _mouseSensitivity;
        
        _parent.rotation = Quaternion.Euler(0f, _rotationY, 0f);
    }
    
    void MoveStop(InputAction.CallbackContext obj)
    {
        _joystickDirection = Vector3.zero;
    }
    
    void Update()
    {
        if (_weapon.IsCurrentlyAttacking)
            return;        
        if (_parry.IsParring)
            return;
        
        var directionFinale = _joystickDirection;
        // On récupère les axes de la cam pour les retravailler et les exploiter dans notre direction
        Vector3 f = _camera.transform.forward;
        f.y = 0f;
        var r = _camera.transform.right;
            
        // Calcul de la direction finale
        var dir = (f * directionFinale.z) + (r * directionFinale.x);
        float staminaFactor = _stamina.IsExhaust ? 0.5f : 1;
        var translation = dir * (_speed * Time.deltaTime * staminaFactor);
        _parent.Translate (translation, Space.World);
        _animator.SetFloat("Speed", translation.normalized.magnitude * _speed);

        // Rotation du personnage
        Vector3 ff = _camera.transform.forward;
        ff.y = 0f;
        _parent.forward = ff;
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
