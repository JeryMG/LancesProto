using System;
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
    public Transform projectileContainer;
    [SerializeField] private float shootingDistanceTreshold = 3f;
    [SerializeField] private float TimeBetweenShots = 0.3f;
    private float NextShotTime;

    [Header("Mode Idle")]
    [SerializeField] private float idleDistanceTreshold = 10f;

    [Header("Patrolling")]
    private Transform[] points;
    public Transform pathHolder;
    private int destPoint = 0;

    [Header("Animations")]
    public ParticleSystem DeathEffect;
    //[SerializeField] private List<RuntimeAnimatorController> Anim =new List<RuntimeAnimatorController>();
    private Animator animPerso;
    public Animator gongWaveAnimator;

    
    protected override void Start() 
    {
        base.Start();
        animPerso = GetComponent<Animator>();
        //animPerso.Play("AnimVolEnn");
        
        pathFinder = GetComponent<NavMeshAgent>();
        Onde.SetActive(false);

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Patrolling;
            hasTarget = true;
            _hunter = FindObjectOfType<Hunter>();
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetVie = _hunter.GetComponent<Vivant>();

            StartCoroutine(UpdatePath());
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
        float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
        if (sqrDstToTarget > Mathf.Pow(idleDistanceTreshold, 2))
        {
            currentState = State.Patrolling;
        }
        else
        {
            currentState = State.Chasing;
        }

        //Shoot ici
        if (hasTarget)
        {
            if (Time.time > NextShotTime)
            {
                if (sqrDstToTarget < Mathf.Pow(shootingDistanceTreshold, 2))
                {
                    //currentState = State.Shooting;
                    NextShotTime = Time.time + TimeBetweenShots;
                    StartCoroutine(shoot());
                }
            }
        }

        if (currentState == State.Idle)
       {
           pathFinder.enabled = false;
       }

       if (currentState == State.Chasing)
       {
           pathFinder.acceleration = 8;
           pathFinder.stoppingDistance = shootingDistanceTreshold;
       }

       if (currentState == State.Patrolling)
       {
           pathFinder.acceleration = 1;
           pathFinder.stoppingDistance = 0;
           if (!pathFinder.pathPending && pathFinder.remainingDistance < 0.5f)
               GotoNextPoint();
       }
    }

    IEnumerator shoot()
    {
        GameObject newProjectile = Instantiate(prefabProjectile, arcPoint.position, Quaternion.Euler(90,0,0), projectileContainer);
        newProjectile.transform.LookAt(_hunter.transform.position);
        
        yield return null;
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
