using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f;
    public float Health { get; private set;}
    public bool IsDeath {get; private set;}
    public UnityEvent OnDead;

    protected virtual void OnEnable()
    {
        IsDeath = false;
        Health = startingHealth;
    }
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDeath)
        {
            return;
        }

        Health -= damage;

        if (Health <= 0)
        {
            Health = 0f;
            Die();
        }
    }

    public virtual void Die()
    {
        IsDeath = true;
        OnDead.Invoke();
    }
}
