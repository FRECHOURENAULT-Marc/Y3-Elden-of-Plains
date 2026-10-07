using System.Collections.Generic;
using UnityEngine;
using Deps = DependenciesInjector;

public class ParticulePlayer : MonoBehaviour
{
    public void PlayEffect(string effectName)
    {
        GameObject effect = Deps.PartDeps.GetPrefab(effectName);
        if (effect == null)
            return;
        Instantiate(effect, transform.position, transform.rotation);
    }
}
