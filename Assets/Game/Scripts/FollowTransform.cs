using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] float _refreshTime = 0.5f;
    [SerializeField] UnityEvent _onDestinationReached;
    
    bool _isStopped = false;

    void Reset()
    {
        if(_agent ==null) _agent = GetComponent<NavMeshAgent>();
    }
    
    void Start()
    {
        if (_target == null || _agent == null)
        {
            Debug.LogError("FollowTransform: No target or NavMeshAgent found");
            Destroy(this);
            return;
        }
        
        RefreshTargetAsync(destroyCancellationToken);
    }

    public void StopFollowing()
    {
        _isStopped = true;
        _agent.ResetPath();
    }

    public void ResumeFollowing()
    {
        _isStopped = false;
        _agent.SetDestination(_target.position);
    }

    public float GetDistanceToTarget()
    {
        return (_target.position - transform.position).magnitude;
    }

    async void RefreshTargetAsync(CancellationToken cancel)
    {
        float startTime = Time.time;
        float timer = 0.0f;
        // while (destroyCancellationToken.IsCancellationRequested == false)
        while (true)
        {
            await Awaitable.NextFrameAsync(cancel);
            if (cancel.IsCancellationRequested)
                return;

            
            if (_isStopped)
                continue;
            
            if (_agent.isStopped)
                _onDestinationReached?.Invoke();
            
            timer = Time.time - startTime;

            if (timer < _refreshTime) continue;
            
            startTime = Time.time;
            _agent.SetDestination(_target.position);
        }
    }
}
