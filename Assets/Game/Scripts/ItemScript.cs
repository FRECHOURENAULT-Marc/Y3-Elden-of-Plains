using UnityEngine;
using UnityEngine.Animations;

public class ItemScript : MonoBehaviour
{
    Transform _pTrs;
    ConstraintSource _cs;
    ParentConstraint _constraint;
    Collider _collider;
    Rigidbody _rb;

    void Start()
    {
        _pTrs = DependenciesInjector.PDeps.Player.transform;
        _cs = new ConstraintSource();
        _constraint = GetComponent<ParentConstraint>();
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
        
        _constraint.constraintActive = false;
        
        _cs.sourceTransform = DependenciesInjector.PDeps.Hand.transform;
        _cs.weight = 1;
        _constraint.AddSource(_cs);
    }
    
    public void Take()
    {
        Debug.Log("Take: " + gameObject.name);
        
        transform.rotation = Quaternion.identity;
        
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
