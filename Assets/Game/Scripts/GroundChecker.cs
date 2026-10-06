using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    bool _isGrounded;
    
    public bool IsGrounded
    {
        get => _isGrounded;
    }
    
    bool ColliderHasGroundTag(Collision other) 
        => other.gameObject.TryGetComponent(out GroundTag gt);
  
    bool ColliderHasGroundTag(Collider other) 
        => other.gameObject.TryGetComponent(out GroundTag gt);
    
    void OnTriggerEnter(Collider other)
    {
        if (ColliderHasGroundTag(other) == false)
            return;

        _isGrounded = true;
    }    
    void OnTriggerExit(Collider other)
    {
        if (ColliderHasGroundTag(other) == false)
            return;
        
        _isGrounded = false;
    }
}
