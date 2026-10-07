using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class UIEnnemiLife : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] float _showDistance;
    [SerializeField] List<GameObject> _uiChildren;

    [ShowNonSerializedField] Transform _worldAnchor;
    [ShowNonSerializedField] RectTransform _uiRect;
    Camera _cam;
    
    public Transform WorldAnchor { get => _worldAnchor; set => _worldAnchor = value; }
    
    Vector3 _originalScale;
    
    void Reset()
    {
        _uiRect = GetComponent<RectTransform>();
        _showDistance = 20.0f;
        _cam = Camera.main;
    }
    
    void Start()
    {
        if(_showDistance <= 0)
            _showDistance = 20.0f;
        
        if(_cam == null)
            _cam = Camera.main;
        
        if(_uiRect == null)
            _uiRect = GetComponent<RectTransform>();
        if (_uiRect == null || _cam == null)
        {
            Debug.LogError("UIEnnemiLife has empty requirements");
            return;
        }
        
        _originalScale = _uiRect.localScale;
    }

    void FixedUpdate()
    {
        float dist = Vector3.Distance(_cam.transform.position, _worldAnchor.position);
        if (dist > _showDistance)
        {
            foreach (GameObject uiChild in _uiChildren)
                uiChild.SetActive(false);
            return;
        }
            
        foreach (GameObject uiChild in _uiChildren)
            uiChild.SetActive(true);
        
        float scaleFactor = (_showDistance - dist + dist*0.2f) / _showDistance;
        scaleFactor = Math.Clamp(scaleFactor, 0, 1);
        
        _uiRect.localScale = _originalScale * scaleFactor;
    }
}
