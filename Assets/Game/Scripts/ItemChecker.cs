using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemChecker : MonoBehaviour
{
    //If input is using, set rb of Item always on Kinematic = true
    
    [SerializeField] ItemScript _currentItem;
    
    public ItemScript Item {get => _currentItem;} 
    
    /// <summary>
    /// first is old item, second is the new one
    /// </summary>
    public event Action<ItemScript, ItemScript> OnTake;
    
    void HandleCollider(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ItemScript item) == false)
            return;
        if (item == _currentItem)
            return;
        if (_currentItem)
            _currentItem.Release();
        var oldItem = _currentItem;
        _currentItem = item;
        item.Take();
        OnTake?.Invoke(oldItem, _currentItem);
    }
    
    // For potential input
    // private void OnTriggerStay(Collider other)
    // private void OnCollisionStay(Collision other)
    
    private void OnTriggerEnter(Collider other) => HandleCollider(other);
    private void OnCollisionEnter(Collision collision) => HandleCollider(collision.collider);
}
