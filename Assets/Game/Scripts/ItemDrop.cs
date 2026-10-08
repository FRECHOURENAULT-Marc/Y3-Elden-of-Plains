using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    
    [SerializeField] GameObject _lootPrefab;

    public void DropItem(Vector3 position)
    {
        if (_lootPrefab == null)
            return;
        Instantiate(_lootPrefab, position, Quaternion.identity);
    }
}
