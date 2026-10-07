using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBloodOpacity : MonoBehaviour
{
    [SerializeField] float _showTime;
    CancellationTokenSource _cs;
    LivingEntity _life;
    Image _uiImage;

    void Reset()
    {
        _showTime = 3.0f;
        _uiImage = GetComponent<Image>();
    }
    
    void Start()
    {
        _life = DependenciesInjector.PDeps.Life;
        _uiImage = GetComponent<Image>();
        _cs = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        _life.OnDamaged += HandleOpacity;
        _life.OnDeath += () => SetAlpha(1);
        _life.OnRespawn += () => SetAlpha(0);

        SetAlpha(0);
    }

    void SetAlpha(float alpha)
    {
        Color nColor = _uiImage.color;
        nColor.a = alpha;
        _uiImage.color = nColor;
    }

    async void HandleOpacity()
    {
        _cs.Cancel();
        _cs = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

        if (_life.Life == 0)
            return;
        
        float lifeFactor = (1.0f - _life.Life / _life.MaxLife);
        float timerFactor = 1.0f + (1.0f - _life.Life / _life.MaxLife);
        SetAlpha(lifeFactor);

        float totalTime = _showTime * timerFactor;
        float timer = 0;
        float startTime = Time.time;
        while (timer < totalTime)
        {
            await Awaitable.NextFrameAsync(_cs.Token);
            if (_cs.Token.IsCancellationRequested)
                return;
            timer = Time.time - startTime;
            float timeFactor = (1.0f - timer/totalTime);
            SetAlpha(lifeFactor * timeFactor);
        }
    }
}
