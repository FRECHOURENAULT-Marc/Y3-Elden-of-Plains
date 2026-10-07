using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    
    [SerializeField] GameObject _lootPrefab;

    public void DropItem(Vector3 position)
    {
        Instantiate(_lootPrefab, position, Quaternion.identity);
    }
}
