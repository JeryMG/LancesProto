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
        Shooting,
        Gonging,
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
    public float stopingD = 8f;
    public float shootingDistanceTreshold = 3f;
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
    private Anim_E_Vole AnimVole;
    public Animator gongWaveAnimator;
    public bool Echojoue=false;
    public bool facing;


    protected override void Start() 
    {
        base.Start();

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
        if(Echojoue==true&&AnimVole.anim.GetCurrentAnimatorStateInfo(0).normalizedTime>=1)
            {
                AnimVole.AnimVole();
                Echojoue=false;
            }
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
                if (sqrDstToTarget <= Mathf.Pow(shootingDistanceTreshold, 2) /*&& sqrDstToTarget >= Mathf.Pow(stopingD, 2)*/)
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
            //pathFinder.stoppingDistance = stopingD;
            pathFinder.acceleration = 8;
        }

        if (currentState == State.Patrolling)
        {
            pathFinder.acceleration = 1;
            pathFinder.stoppingDistance = 0;
            if (!pathFinder.pathPending && pathFinder.remainingDistance < 0.5f)
                GotoNextPoint();
        }

        if (currentState == State.Gonging)
        {
            propagOnde();
        }
    }

    IEnumerator shoot()
    {
        yield return StartCoroutine(TurnToFace(target.position));
        currentState = State.Shooting;
        pathFinder.enabled = false;
        
        GameObject newProj = Instantiate(prefabProjectile, arcPoint.position, arcPoint.rotation);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/Tir",transform.position);
        NextShotTime = Time.time + TimeBetweenShots;
        
        yield return null;
        currentState = State.Chasing;
        pathFinder.enabled = true;
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
    
    IEnumerator TurnToFace(Vector3 looktarget)
    {
        if (!facing)
        {
            Debug.Log("YOOOOOO");
            pathFinder.isStopped = true;
            Vector3 dirToLookTarget = (looktarget - transform.position).normalized;
            float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

            while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
            {
                float turnSpeed = 360;
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
                transform.eulerAngles = Vector3.up * angle;
                yield return null;
            }
            pathFinder.isStopped = false;
            facing = true;
        }
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
        gongWaveAnimator.SetTrigger("Elargi");
        FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/Cloches/Ennemi_Cloche",transform.position);
        AnimVole.AnimVoleEchos();
        Echojoue=true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wave"))
        {
            currentState = State.Gonging;
        }
    }
}
