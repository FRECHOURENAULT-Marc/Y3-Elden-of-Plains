using Unity.Cinemachine;
using UnityEngine;
using Deps = DependenciesInjector;

public class CameraTrackingTargetPlayer : MonoBehaviour
{
    void Start()
    {
        GetComponent<CinemachineCamera>().Target.TrackingTarget = Deps.PDeps.Player.transform;
    }
}
