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
    public GameObject HitParticule;

    public event Action OnDeath;

    protected virtual void Start()
    {
        health = MaxHealth;
    }
    
    public virtual void TakeDamage(float damage)
    {
        Destroy(
            Instantiate(HitParticule, transform.position,
                Quaternion.FromToRotation(Vector3.forward, transform.position)),
            2);
        
        // Son Hit Effect
/*        if (this.CompareTag("Player"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Hit_Effect/Hit_Effect");
        }*/

        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
        //  HitParticule.SetActive(false);


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
