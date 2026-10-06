using UnityEngine;
using UnityEngine.UI;

public class UIPlayerParryReload : MonoBehaviour
{
    [SerializeField] RectMask2D _uiMask;
    
    Parade _parry;
    float _timer;
    float _sizeFactor;

    void Start()
    {
        _parry = DependenciesInjector.PDeps.Parry;
        
        _parry.OnParryStart += () => SetReload(0);
        _parry.OnParryEnd += () => StartTimer();
        
        _sizeFactor = _uiMask.rectTransform.rect.height / 100;
        _sizeFactor *= _uiMask.rectTransform.lossyScale.y;
    }

    void SetReload(float percentage)
    {
        Vector4 pad = _uiMask.padding;
        pad.w = _sizeFactor * (100 - percentage); // W = Top

        _uiMask.padding = pad;
    }

    async void StartTimer()
    {
        _timer = 0;
        float startTime = Time.time;
        float dt = Time.time;
        while (_timer < _parry.RecoveryTime)
        {
            await Awaitable.NextFrameAsync(destroyCancellationToken);
            if (destroyCancellationToken.IsCancellationRequested)
                return;
            
            dt = Time.time - startTime;
            _timer += dt;
            
            SetReload((_timer / _parry.RecoveryTime) * 100);
        }
    }
}
