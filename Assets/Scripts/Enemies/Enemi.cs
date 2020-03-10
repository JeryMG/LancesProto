using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemi : Vivant
{
    public enum State
    {
        Idle,
        Chasing,
        Attacking,
    }

    public ParticleSystem DeathEffect;

    private bool hasTarget;

    public State currentState;
    private NavMeshAgent pathFinder;
    private Hunter _hunter;
    private Transform target;
    private Vivant targetVie;
    private Material skinMat;
    private Color originalColor;
    
    //Variables en mode attack cac
    private float attackDistanceTreshold = 3f;
    private float TimeBetweenAttacks = 1;
    private float NextAttackTime;
    private float myCollisionRadius;

    public float damage = 2f;
    [SerializeField] private float idleDistanceTreshold = 10f;
    private bool stunned;

    protected override void Start()
    {
        base.Start();
        OnDeath += deathEffect;
        pathFinder = GetComponent<NavMeshAgent>();
        skinMat = GetComponent<Renderer>().material;
        originalColor = skinMat.color;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Idle;
            _hunter = FindObjectOfType<Hunter>();
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetVie = _hunter.GetComponent<Vivant>();
            targetVie.OnDeath += OnTargetDeath;
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        
            StartCoroutine(UpdatePath());
        }
    }

    private void Update()
    {
        float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
        if (sqrDstToTarget > Mathf.Pow(idleDistanceTreshold, 2) || stunned)
        {
            currentState = State.Idle;
        }
        else
        {
            currentState = State.Chasing;
        }

        if (hasTarget)
        {
            if (Time.time > NextAttackTime)
            {
                if (sqrDstToTarget < Mathf.Pow(attackDistanceTreshold, 2))
                {
                    NextAttackTime = Time.time + TimeBetweenAttacks;
                    //StartCoroutine(Attack());
                }
            }
        }
        
        if (currentState == State.Idle)
        {
            pathFinder.enabled = false;
            return;
        }
    }

    public IEnumerator UpdatePath()
        {
            float refreshRate = 0.25f;

            while (hasTarget)
            {
                if (currentState == State.Chasing)
                {
                    pathFinder.enabled = true;
                    Vector3 targetPosition = new Vector3(target.position.x, 0 , target.position.z);
                    if (!dead)
                    {
                        pathFinder.SetDestination(targetPosition);
                    }
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        skinMat.color = Color.red;
        //pathFinder.enabled = false;
    
        Vector3 originalPosition = transform.position;
        Vector3 attackPosition = target.position - (target.position - transform.position).normalized * myCollisionRadius;
        float attackSpeed = 3f;
        float percent = 0;
        bool appliedDamage = false;
    
        //"Animation" de Lunge
        while (percent <= 1)
        {
            if (percent >= .5f && !appliedDamage)
            {
                appliedDamage = true;
                targetVie.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;
        }
        skinMat.color = originalColor;
        currentState = State.Chasing;
        //pathFinder.enabled = true;
    }

    void deathEffect()
    {
        Destroy(
            Instantiate(DeathEffect, transform.position,
                Quaternion.FromToRotation(Vector3.forward, transform.position)),
            DeathEffect.main.startLifetimeMultiplier);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Lance"))
        {
            if (health <= 0)
            {
                Destroy(gameObject);
                Destroy(
                    Instantiate(DeathEffect, transform.position,
                        Quaternion.FromToRotation(Vector3.forward, transform.position)),
                    DeathEffect.main.startLifetimeMultiplier);
            }
        }
    }

    void OnTargetDeath()
        {
            hasTarget = false;
            currentState = State.Idle;
        }

    public IEnumerator stun(float time)
    {
        stunned = true;
        Debug.Log("Stunned");
        yield return new WaitForSeconds(time);
        stunned = false;
    }
}
