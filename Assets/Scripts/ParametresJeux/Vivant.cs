using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Schema;

public class Vivant : MonoBehaviour, IDamageable
{
    [SerializeField]protected float health;
    [SerializeField] private float MaxHealth = 5;
    public bool stunned;
    protected bool dead;

    public event Action OnDeath;

    protected virtual void Start()
    {
        health = MaxHealth;
    }
    
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public IEnumerator StunXseconds(float seconds)
    {
        if (stunned)
        {
            yield return new WaitForSeconds(seconds);
            stunned = false;
        }
    }
    
    [ContextMenu("Se suicider")]
    protected void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        Destroy(gameObject);
    }
}
