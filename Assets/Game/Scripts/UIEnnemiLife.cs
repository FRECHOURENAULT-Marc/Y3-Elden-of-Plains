using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnnemiLife : MonoBehaviour
{

    [Header("Dependencies")]
    [SerializeField] Transform _worldAnchor;
    [SerializeField] RectTransform _uiRect;
    [SerializeField] Camera _cam;
    
    [Header("Configuration")]
    [SerializeField] float _showDistance;
    [SerializeField] List<GameObject> _uiChildren;

    Vector3 _originalScale;
    
    void Reset()
    {
        _uiRect = GetComponent<RectTransform>();
        _showDistance = 30.0f;
    }

    public void Init(Transform worldAnchor)
    {
        _worldAnchor = worldAnchor;
        _cam = Camera.main;
    }
    void Start()
    {
        if (_uiChildren == null || _uiRect == null || _worldAnchor == null)
        {
            Debug.LogError("Missing dependencies for UIEnnemiLife");
            Destroy(this);
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
        
        Vector2 screenPos= _cam.WorldToScreenPoint(_worldAnchor.position);
        _uiRect.position = screenPos;
        
        
    }
}
