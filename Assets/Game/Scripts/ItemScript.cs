using UnityEngine;
using UnityEngine.Animations;

public class ItemScript : MonoBehaviour
{
    Transform _pTrs;
    ParentConstraint _constraint;
    Collider _collider;
    Rigidbody _rb;

    void Start()
    {
        _pTrs = DependenciesInjector.PDeps.Player.transform;
        _constraint = GetComponent<ParentConstraint>();
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
    }
    
    public void Take()
    {
        Debug.Log("Take: " + gameObject.name);
        
        _constraint.constraintActive = true;
        _collider.isTrigger = true;
        _rb.isKinematic = true;
    }    
    public void Release()
    {
        transform.parent = null;
        
        _constraint.constraintActive = false;
        _collider.isTrigger = false;
        _rb.isKinematic = false;
        
        transform.rotation = Quaternion.identity;
        transform.position += _pTrs.forward * 3;
    }
}
