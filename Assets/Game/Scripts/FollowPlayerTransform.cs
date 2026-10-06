using UnityEngine;
using UnityEngine.AI;

public class FollowPlayerTransform : FollowTransform
{
    new void Start()
    {
        _targetCollider = DependenciesInjector.PDeps.Collider.transform;
        _agent = transform.parent.parent.GetComponent<NavMeshAgent>();
        
        base.Start();
    }
}
