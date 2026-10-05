using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Reach a target with NavMeshAgent on a NavMeshSurface
/// Stop when the attached collider touch the target collider
/// </summary>
public class FollowTransform : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] float _refreshTime;
    [SerializeField] UnityEvent _onDestinationReached;
    
    bool _isStopped;
    public bool IsStopped { get => _isStopped; }

    public Transform Target { get => _target; }

    void Reset()
    {
        if(_agent ==null) _agent = GetComponent<NavMeshAgent>();
        _refreshTime = 0.5f;
    }
    
    void Start()
    {
        if (_target == null || _agent == null)
        {
            Debug.LogError("FollowTransform: No target or NavMeshAgent found");
            Destroy(this);
            return;
        }
        
        _isStopped = false;
        
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
        while (true)
        {
            await Awaitable.WaitForSecondsAsync(_refreshTime, cancel);
            
            if (_isStopped)
                continue;
            
            if (_agent.isStopped)
                _onDestinationReached?.Invoke();
            
            _agent.SetDestination(_target.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform != _target)
            return;
        
        StopFollowing();
    }    
    void OnTriggerExit(Collider other)
    {
        if (other.transform != _target)
            return;
        
        ResumeFollowing();
    }
}
