using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml.Schema;

public class Vivant : MonoBehaviour, IDamageable
{
    [SerializeField]protected float health;
    [SerializeField] private float MaxHealth = 5;
    public bool stunned;
    public bool dead;
    public GameObject HitParticule;
    [Header("HP variables")]
    public Image LifeFill;
    private float lifeAmount;
    private bool isLifeFillNotNull;
    private bool goRestoreHP;
    private bool restoringHP;


    public event Action OnDeath;

    private void Awake()
    {
        isLifeFillNotNull = LifeFill != null;
    }

    protected virtual void Start()
    {
        health = MaxHealth;
        if (isLifeFillNotNull)
        {
            LifeFill.fillAmount = 1f;
        }
    }
    
    public virtual void TakeDamage(float damage)
    {
        Destroy(
            Instantiate(HitParticule, transform.position,
                Quaternion.FromToRotation(Vector3.forward, transform.position)),
            2);

        health -= damage;
        if (CompareTag("Player"))
        {
            //goRestoreHP = true;
            UpdateLifeBar();
            //StartCoroutine(restoreHP());
        }
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    private void UpdateLifeBar()
    {
        lifeAmount = health / MaxHealth;
        LifeFill.fillAmount = lifeAmount;
    }

    IEnumerator restoreHP()
    {
        if (goRestoreHP)
        {
            yield return new WaitForSeconds(10);
            restoringHP = true;
            goRestoreHP = false;
        }
        
        if (health < MaxHealth)
        {
            if (restoringHP)
            {
                health += 0.5f;
                UpdateLifeBar();
                yield return new WaitForSeconds(1);
            }
        }
        else
        {
            yield return null;
            restoringHP = false;
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
