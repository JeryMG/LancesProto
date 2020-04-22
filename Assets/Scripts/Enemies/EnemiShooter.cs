﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemiShooter : Vivant, IClochePropag
{
    public enum State
    {
        Idle,
        Chasing,
        Patrolling,
        Shooting,
    }

    public State currentState;
    public GameObject Onde;
    private NavMeshAgent pathFinder;
    private bool hasTarget;
    private Hunter _hunter;
    private Transform target;
    private Vivant targetVie;

    [Header("Mode Shooting")]
    //public float damage = 2f;
    public GameObject prefabProjectile;
    public Transform arcPoint;
    [SerializeField] private float stopingD = 8f;
    [SerializeField] private float shootingDistanceTreshold = 3f;
    [SerializeField] private float TimeBetweenShots = 0.3f;
    private float NextShotTime;

    [Header("Mode Idle")]
    [SerializeField] private float idleDistanceTreshold = 10f;

    [Header("Patrolling")]
    private Transform[] points;
    public Transform pathHolder;
    private int destPoint = 0;
    public float turnSpeed = 90f;

    [Header("Animations")]
    public ParticleSystem DeathEffect;
    //[SerializeField] private List<RuntimeAnimatorController> Anim =new List<RuntimeAnimatorController>();
    private Animator animPerso;
    private Anim_E_Vole AnimVole;
    public Animator gongWaveAnimator;
    private FollowPlayer followPlayer;


    protected override void Start() 
    {
        base.Start();
        followPlayer = GetComponent<FollowPlayer>();

        animPerso = GetComponent<Animator>();
        //animPerso.Play("AnimVolEnn");
        AnimVole = GetComponentInChildren<Anim_E_Vole>();
        
        pathFinder = GetComponent<NavMeshAgent>();
        Onde.SetActive(false);

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Patrolling;
            hasTarget = true;
            _hunter = FindObjectOfType<Hunter>();
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetVie = _hunter.GetComponent<Vivant>();
        }
        
        points = new Transform[pathHolder.childCount];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = pathHolder.GetChild(i);
            points[i].position = pathHolder.GetChild(i).position;
            points[i].position = new Vector3(points[i].position.x, transform.position.y, points[i].position.z);
        }

        OnDeath += enemyDeath;
    }

    private void Update() 
    {
        if (shootingDistanceTreshold < stopingD)
        {
            shootingDistanceTreshold = stopingD;
        }
        float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
        if (sqrDstToTarget > Mathf.Pow(idleDistanceTreshold, 2))
        {
            currentState = State.Patrolling;
        }
        if(sqrDstToTarget < Mathf.Pow(idleDistanceTreshold, 2))
        {
            currentState = State.Chasing;
        }
        
        if (hasTarget)
        {
            if (Time.time > NextShotTime)
            {
                if (sqrDstToTarget < Mathf.Pow(shootingDistanceTreshold, 2))
                {
                    NextShotTime = Time.time + TimeBetweenShots;
                    StartCoroutine(shoot());
                }
            }
        }
        
        if (currentState == State.Idle)
        {
            pathFinder.enabled = false;
            AnimVole.AnimVole();
        }

        if (currentState == State.Chasing)
        {
            pathFinder.stoppingDistance = stopingD;
            pathFinder.acceleration = 8;
        }

        if (currentState == State.Patrolling)
        {
            pathFinder.acceleration = 1;
            pathFinder.stoppingDistance = 0;
            if (!pathFinder.pathPending && pathFinder.remainingDistance < 0.5f)
                GotoNextPoint();
        }
        //anime marche lente
    }

    IEnumerator shoot()
    {
        currentState = State.Shooting;
        pathFinder.enabled = false;
        
        GameObject newProj = Instantiate(prefabProjectile, arcPoint.position, arcPoint.rotation);
        NextShotTime = Time.time + TimeBetweenShots;
        
        yield return null;
        currentState = State.Chasing;
        pathFinder.enabled = true;
    }
    
    IEnumerator TurnToFace(Vector3 looktarget)
    {
        Vector3 dirToLookTarget = (looktarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    void GotoNextPoint() 
    {
        // Returns si ya pas de points dans le tableaux
        if (points.Length == 0)
        {
            currentState = State.Idle;
            return;
        }

        // Set la destination au point actuel selectionné du tableau
        pathFinder.SetDestination(points[destPoint].position);

        // Choisir prochain point,
        // revenir au debut du tableau quand on arrive au bout.
        destPoint = (destPoint + 1) % points.Length;
    }

    void enemyDeath()
    {
        //son de destruction 
        FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/DestructionEnnemi",transform.position);
        
        //Animation de mort
        
        
        Destroy(
            Instantiate(DeathEffect, transform.position,
                Quaternion.FromToRotation(Vector3.forward, transform.position)),
            DeathEffect.main.startLifetimeMultiplier);
    }

    [ContextMenu("propage onde")]
    public void propagOnde()
    {
        Onde.SetActive(true);
        gongWaveAnimator.SetTrigger("Elargi");
        Invoke("desactiveOnde", 3f);
        AnimVole.AnimVoleEchos();

    }

    private void desactiveOnde()
    {
        Onde.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wave"))
        {
            propagOnde();
        }
    }
}
