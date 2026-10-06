using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemChecker : MonoBehaviour
{
    //If input is using, set rb of Item always on Kinematic = true
    
    [SerializeField] ItemScript _currentItem;
    
    void HandleCollider(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ItemScript item) == false)
            return;
        if (item == _currentItem)
            return;
        _currentItem.Release();
        _currentItem = item;
        item.Take();
    }
    
    // For potential input
    // private void OnTriggerStay(Collider other)
    // private void OnCollisionStay(Collision other)
    
    private void OnTriggerEnter(Collider other) => HandleCollider(other);
    private void OnCollisionEnter(Collision collision) => HandleCollider(collision.collider);
}
