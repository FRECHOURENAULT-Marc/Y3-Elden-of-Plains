using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = System.Object;

public class PlayerController : MonoBehaviour
{
    [Header("Dependances")] 
    [SerializeField] Rigidbody _rb;
    [SerializeField] Camera _camera;
    [SerializeField] InputActionReference _moveInput;
    [SerializeField] InputActionReference _jumpInput;
        
    [Header("Conf")]
    [SerializeField] float _speed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _mouseSensitivity;
    
    bool _isGrounded;
    Vector3 _joystickDirection;
    private float _rotationY;
    
    void Reset()
    {
        _speed = 10f;
        _rb = GetComponent<Rigidbody>();
        _jumpForce = 100f;
    }
    
    void Start()
    {
        _moveInput.action.started += MoveUpdate;
        _moveInput.action.performed += MoveUpdate;
        _moveInput.action.canceled += MoveStop;
        
        _jumpInput.action.started += Jump;
    }

    void MoveUpdate(InputAction.CallbackContext obj)
    {
        _joystickDirection = obj.ReadValue<Vector2>();
        _joystickDirection = new Vector3(_joystickDirection.x, 0f, _joystickDirection.y);
    }
    void Jump(InputAction.CallbackContext obj)
    {
        Debug.Log("Jump");
        
        if (_isGrounded)
            _rb.AddForce(Vector3.up * _jumpForce);
    }

    void HandleRotation() // En attente
    {
        float mouseX = Input.GetAxis("Mouse X");
        _rotationY += mouseX * _mouseSensitivity;
        
        transform.rotation = Quaternion.Euler(0f, _rotationY, 0f);
    }
    
    void MoveStop(InputAction.CallbackContext obj)
    {
        _joystickDirection = Vector3.zero;
    }
    
    void Update()
    {
        var directionFinale = _joystickDirection;
        // On récupère les axes de la cam pour les retravailler et les exploiter dans notre direction
        Vector3 f = _camera.transform.forward;
        f.y = 0f;
        var r = _camera.transform.right;
            
        // Calcul de la direction finale
        var dir = (f * directionFinale.z) + (r * directionFinale.x);
        transform.Translate (dir * (_speed * Time.deltaTime), Space.World);

        // Rotation du personnage
        Vector3 ff = _camera.transform.forward;
        ff.y = 0f;
        transform.forward = ff;
    }

    bool ColliderHasGroundTag(Collision other) 
        => other.gameObject.TryGetComponent(out GroundTag gt);
  
    // bool ColliderHasGroundTag(Collider other) 
    //     => other.gameObject.TryGetComponent(out GroundTag gt);
    
    void OnCollisionEnter(Collision other)
    {
        if (ColliderHasGroundTag(other) == false)
            return;

        _isGrounded = true;
        Debug.Log("Grounded");
    }    
    void OnCollisionExit(Collision other)
    {
        if (ColliderHasGroundTag(other) == false)
            return;
        
        _isGrounded = false;
        Debug.Log("Grounded");
    }


}
