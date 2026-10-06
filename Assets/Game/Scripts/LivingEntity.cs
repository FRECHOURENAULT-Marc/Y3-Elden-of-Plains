using System;
using UnityEngine;

public class LivingEntity : LifeScript
{
    bool _isInvincible;
    
    public event Action OnRespawn;
    public event Action OnDeath;
    
    public bool IsInvincible
    {
        get => _isInvincible;
        set => _isInvincible = value;
    }

    public override void ApplyDamage(float damage)
    {
        bool alreadyDead = !IsAlive;
        
        base.ApplyDamage(damage);
        
        if (IsAlive) return;
        if (alreadyDead) return;
        
        OnDeath?.Invoke();
    }
    
    public override void TryDamaging(float damage)
    {
        if (IsInvincible) return;
        
        base.TryDamaging(damage);
    }
    
    public void Respawn()
    {
        Life = MaxLife;
        OnRespawn?.Invoke();
    }
}
