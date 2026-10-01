using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = System.Object;

public class PlayerController : MonoBehaviour
{
    [Header("Dependances")] 
    [SerializeField] Rigidbody _rb;
    [SerializeField] InputActionReference _moveInput;
    [SerializeField] InputActionReference _jumpInput;
    [SerializeField] Camera _camera;
        
    [Header("Conf")]
    [SerializeField] Vector3 _direction;
    [SerializeField] float _speed;
    [SerializeField] float _jumpForce;
    [SerializeField] GameObject _footPosition;
    [SerializeField] float _mouseSensitivity;
    
    bool _isGrounded;
    Vector3 _joystickDirection;
    private float _rotationY;
    
    void Reset()
    {
        _direction = Vector3.forward;
        _speed = 10f;
        _rb = GetComponent<Rigidbody>();
        _jumpForce = 100f;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start");
        
        _moveInput.action.started += MoveUpdate;
        _moveInput.action.performed += MoveUpdate;
        _moveInput.action.canceled += MoveStop;
    }

    void MoveUpdate(InputAction.CallbackContext obj)
    {
        _joystickDirection = obj.ReadValue<Vector2>();
        _joystickDirection = new Vector3(_joystickDirection.x, 0f, _joystickDirection.y);
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        _rotationY += mouseX * _mouseSensitivity;
        
        transform.rotation = Quaternion.Euler(0f, _rotationY, 0f);
    }
    
    void MoveStop(InputAction.CallbackContext obj)
    {
        _joystickDirection=Vector3.zero;
    }
    
    // Update is called once per frame
    void Update()
    {
        // Detection du sol basique
        _isGrounded = Physics.Raycast(_footPosition.transform.position, Vector3.down, 1f);
        Debug.DrawRay(_footPosition.transform.position, Vector3.down *1, Color.red);
        
        if (_jumpInput.action.WasPressedThisFrame()) // && _isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpForce);
        }
        
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
    
    void OnCollisionEnter(Collision other)
    {
        // Detection via un TAG
        //if (other.gameObject.CompareTag("SOL"))
        //{
        //    Debug.Log("Game OVER");
        //}
        
        // Detection avec component
        var gt = other.gameObject.GetComponent<GroundTag>();
        if (gt != null)
        {
            Debug.Log("Game OVER");
        }

        if (other.gameObject.TryGetComponent(out GroundTag gtt))
        {
            Debug.Log("Game OVER");
        }
        
    }
}
