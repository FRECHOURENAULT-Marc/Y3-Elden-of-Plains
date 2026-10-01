using System;
using NaughtyAttributes;
using UnityEngine;

public class LifeScript : MonoBehaviour
{
    public enum GO_TEAM
    {
        Player,
        Enemies,
        Other
    }
    
    [SerializeField] private GO_TEAM _team;
    [SerializeField] private float _maxLife;
    [ShowNonSerializedField] private float _life;
    
    public event Action OnDamaged;
    
    public GO_TEAM GoTeam
    {
        get => _team;
        protected set => _team = value;
    }
    public float MaxLife
    {
        get => _maxLife;
        protected set => _maxLife = value;
    }
    public float Life
    {
        get => _life;
        protected set => _life = value;
    }
    public bool IsAlive
    {
        get => Life > 0;
    }
    
    void Reset()
    {
        GoTeam = GO_TEAM.Other;
        MaxLife = 5f;
        Life = MaxLife;
    }

    void Start()
    {
        Life = MaxLife;
    }

    public virtual void ApplyDamage(float damage)
    {
        Life -= damage;
        OnDamaged?.Invoke();
        
        Debug.Log($"[LifeScript] Dealing {damage} damage");
    }
    
    public virtual void TryDamaging(float damage)
    {
        if (IsAlive == false) return;
        
        ApplyDamage(damage);
    }
}
