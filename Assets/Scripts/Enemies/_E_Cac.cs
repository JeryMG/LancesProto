using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class _E_Cac : Vivant, IClochePropag
{
    public enum State
    {
        Idle,
        Chasing,
        Patrolling,
        Attacking,
    }
    public bool DejaJoue=false;
    public State currentState;
    public GameObject Onde;
    private NavMeshAgent pathFinder;
    private bool hasTarget;
    private Hunter _hunter;
    private Transform target;
    private Vivant targetVie;
    
    [Header("Mode Attack")]
    public float damage = 2f;
    [SerializeField] private float attackDistanceTreshold = 3f;
    [SerializeField] private float TimeBetweenAttacks = 1;
    private float NextAttackTime;

    [Header("Mode Idle")]
    [SerializeField] private float idleDistanceTreshold = 10f;

    [Header("Patrolling")]
    private Transform[] points;
    public Transform pathHolder;
    private int destPoint = 0;

    [Header("Animations")]
    public ParticleSystem DeathEffect;
    // [SerializeField] private List<RuntimeAnimatorController> Anim =new List<RuntimeAnimatorController>();
    // public Animator animPerso;
    // public Animator gongWaveAnimator;
    public Anim_EnenmieCac animations_cac;

    
    protected override void Start() 
    {
        base.Start(); 
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

        animations_cac = GetComponentInChildren<Anim_EnenmieCac>();
        
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

       if (hasTarget)
       {
           if (Time.time > NextAttackTime)
           {
               if (sqrDstToTarget < Mathf.Pow(attackDistanceTreshold, 2))
               {
                    //Son CAC 
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiCAC3D/CAC",transform.position);
                    

                    NextAttackTime = Time.time + TimeBetweenAttacks;
                   StartCoroutine(Attack());
               }
           }
       }
        
       if (currentState == State.Idle)
       {
           pathFinder.enabled = false;
           
       }

       if (currentState == State.Chasing&&DejaJoue==false)
       {
           //Anim de marche rapîde !!!!!!
        
           animations_cac.MarcheRapide();
           pathFinder.acceleration = 8;
           pathFinder.stoppingDistance = 3;
           DejaJoue=true;
       }

       if (currentState == State.Patrolling&&DejaJoue==true)
       {
           //Anim de marche lente !!!!!!
           animations_cac.MarcheLente();

           pathFinder.acceleration = 1;
           pathFinder.stoppingDistance = 0;
           DejaJoue=false;
           if (!pathFinder.pathPending && pathFinder.remainingDistance < 0.5f)
               GotoNextPoint();
       }

       if (currentState == State.Attacking)
       {
           //anime d'attaque
            animations_cac.Attaque();

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
        pathFinder.enabled = false;
    
        Vector3 originalPosition = transform.position;
        Vector3 attackPosition = target.position - (target.position - transform.position).normalized;
        float attackSpeed = 3f;
        float percent = 0;
        bool appliedDamage = false;
    
        //ANIMATION
        while (percent <= 1)
        {
             if (percent >= .5f && !appliedDamage)
            {
                appliedDamage = true;
                 targetVie.TakeDamage(damage);
            }
             percent += Time.deltaTime * attackSpeed;
            // float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            // transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;
        }
        
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
        //gongWaveAnimator.SetTrigger("Elargi");
        animations_cac.OndeRecus();
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

        if (other.gameObject.CompareTag("Lance"))
        {
            pathFinder.enabled = false;
            animations_cac.DegasRecus();
            pathFinder.enabled = true;
        }
    }
}
