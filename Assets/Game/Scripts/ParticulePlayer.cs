using System;
using System.Collections.Generic;
using UnityEngine;
using Deps = DependenciesInjector;

[Serializable]
public class ParticuleData
{
    public string name;
    public Vector3 position;
    public Vector3 scale = Vector3.one;
}

public class ParticulePlayer : MonoBehaviour
{
    void SpawnEffect(GameObject effect, Vector3 position, Vector3 scale)
    {
        Instantiate(effect, position, transform.rotation)
            .transform.localScale = scale;
    }
    
    public void PlayEffect(string effectName) 
        => PlayEffect(effectName, transform.position);

    public void PlayEffect(string effectName, Vector3 pos) 
        => PlayEffect(effectName, pos, Vector3.one);

    public void PlayEffect(string effectName, Vector3 pos, Vector3 scale)
    {
        GameObject effect = Deps.PartDeps.GetPrefab(effectName);
        if (effect == null)
        {
            if (effectName == "Parry")
                Debug.Log("Parry not found");
            return;
        }
            
        if (effectName == "Parry")
            Debug.Log("Parry found");
        SpawnEffect(effect, pos, scale);
    } 
}
