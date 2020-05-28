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
    public Image DamageRecuImage;
    private bool isImageDmgNull;
    [SerializeField] private float fadeTime = 0.3f;
    private bool isRunning;
    protected bool godMode;

    public event Action OnDeath;

    private void Awake()
    {
        
        isLifeFillNotNull = LifeFill != null;
        isImageDmgNull = DamageRecuImage != null;
    }

    protected virtual void Start()
    {
        health = MaxHealth;
        if (isLifeFillNotNull)
        {
            LifeFill.fillAmount = 1f;
        }

        if (!isImageDmgNull && CompareTag("Player"))
        {
            Color c = DamageRecuImage.color;
            c.a = 0;
            DamageRecuImage.color = c;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        // Son Hit Effect
        if (this.CompareTag("Player"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Hit_Effect/Hit_Effect");
        }
        Destroy(
            Instantiate(HitParticule, transform.position,
                Quaternion.FromToRotation(Vector3.forward, transform.position)),
            2);

        if (godMode == false)
        {
            health -= damage;
        }
        if (CompareTag("Player"))
        {
            //goRestoreHP = true;
            UpdateLifeBar();
            //StartCoroutine(restoreHP());
            if (!isImageDmgNull && !isRunning)
            {
                StartCoroutine(AfficherDmgRecuIN());
            }
        }
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void RecoverHP(float amount)
    {
        health += amount;
        if (CompareTag("Player"))
        {
            //goRestoreHP = true;
            UpdateLifeBar();
            //StartCoroutine(restoreHP());
        }
        if (health >= MaxHealth && !dead)
        {
            health = MaxHealth;
        }
    }

    private void UpdateLifeBar()
    {
        lifeAmount = health / MaxHealth;
        LifeFill.fillAmount = lifeAmount;
    }

    private YieldInstruction fadeInstruction = new YieldInstruction();

    private IEnumerator AfficherDmgRecuIN()
    {
        isRunning = true;
        //fade in
        float elapsedTime = 0.0f;
        Color c = DamageRecuImage.color;
        while (c.a < 0.82f)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime ;
            c.a = Mathf.Lerp(0, 0.82f, elapsedTime / fadeTime);
            DamageRecuImage.color = c;
        }
        //fade out
        StartCoroutine(AfficherDmgRecuOUT());
    }

    public IEnumerator StunXseconds(float seconds)
    {
        if (stunned)
        {
            yield return new WaitForSeconds(seconds);
            stunned = false;
        }
    }
    IEnumerator AfficherDmgRecuOUT()    
    {
        float elapsedTime = 0.0f;
        Color c = DamageRecuImage.color;
        while (c.a > 0f)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime ;
            //c.a = 0.82f - Mathf.Clamp01(elapsedTime / fadeTime);
            c.a = Mathf.Lerp(0.82f, 0, elapsedTime / fadeTime);
            DamageRecuImage.color = c;
        }
        isRunning = false;
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
