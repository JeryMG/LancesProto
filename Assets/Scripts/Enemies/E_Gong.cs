using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class E_Gong : Vivant
{
    public enum State
    {
        //quand le joueur est mort
        Idle,
        //quand le joueur est dans sa range de gong mais hors de sange cac
        Gonging,
        //quand le joueur est hors de sa range gong
        Patrolling,
        //quand le joueur se trouve tres proche (range de cac)
        Attacking,

    }
    
    public State currentState;
    private NavMeshAgent pathFinder;
    private bool hasTarget;
    private Hunter _hunter;
    private Transform target;
    private Vivant targetVie;
    
    [Header("Mode Idle")]
    [SerializeField] private float idleDistanceTreshold = 40f;
    
    [Header("Patrolling")]
    public Transform[] points;
    public Transform pathHolder;
    private int destPoint = 0;
    
    [Header("Mode Attack")]
    public float damage = 2f;
    [SerializeField] private float attackDistanceTreshold = 3f;
    [SerializeField] private float TimeBetweenAttacks = 1.2f;
    private float NextAttackTime;

    [Header("Gong wave")]
    [SerializeField] private float gongTimer = 10f;
    private float nextGongTime;
    public Animator gongWaveAnimator;
    private TestAnimGong AnimGong;
    
    [Header("Animations")]
    public ParticleSystem DeathEffect;
    [SerializeField] private float vision;
    private bool dejaJouee;
    private bool GActiver=false;
    public bool JoueurAuCac=false;
    private int RandAnimCac=1;
    private bool SoundActiveGong=false;
    

    //[SerializeField] private List<AnimatorController> Anim =new List<AnimatorController>();
    //public Animator animPerso;

    protected override void Start()
    {
        base.Start();
        pathFinder = GetComponent<NavMeshAgent>();
        AnimGong = GetComponentInChildren<TestAnimGong>();
        
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Patrolling;
            hasTarget = true;
            _hunter = FindObjectOfType<Hunter>();
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetVie = _hunter.GetComponent<Vivant>();

            //StartCoroutine(UpdatePath());
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
        if(target!=null)
        {
            float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
            if (sqrDstToTarget > Mathf.Pow(idleDistanceTreshold, 2))
            {
                currentState = State.Patrolling;
            }
            if(sqrDstToTarget < Mathf.Pow(idleDistanceTreshold, 2) && sqrDstToTarget > Mathf.Pow(attackDistanceTreshold, 2) && !stunned)
            {
                currentState = State.Gonging;
            }
            if(sqrDstToTarget < Mathf.Pow(attackDistanceTreshold, 2))
            {
                currentState = State.Attacking;
            }
            if(SoundActiveGong==true&&Time.time < nextGongTime)
            {
                 SoundActiveGong=false;
            }

            if (stunned)
            {
                currentState = State.Idle;
            }
             
            //anims
            /*if(JoueurAuCac==true&&AnimGong.anim.GetCurrentAnimatorStateInfo(1).normalizedTime>1)
            {
                AnimGong.animCac();
                GActiver=true;
            }

            if(sqrDstToTarget<Mathf.Pow(vision, 2)&&GActiver==false)
            {
                AnimGong.GongActiver();                
                GActiver=true;
            }
            
            if(GActiver==true&&AnimGong.anim.GetCurrentAnimatorStateInfo(5).normalizedTime>1)
            {
                RandAnimCac=UnityEngine.Random.Range(1,3);

               GActiver=false;
            }*/
        
            //Attack state
            if (hasTarget)
            {
                if (Time.time > NextAttackTime)
                {
                    if (sqrDstToTarget < Mathf.Pow(attackDistanceTreshold, 2))
                    {
                        currentState = State.Attacking;
                        NextAttackTime = Time.time + TimeBetweenAttacks;
                        StartCoroutine(Attack());
                    }
                }
            }
        }
        if (currentState == State.Idle)
        {
            pathFinder.enabled = false;
            //anim repos
        }
        
        if (currentState == State.Gonging)
        {
            if (!dejaJouee)
            {
                //AnimGong.Marche();
                dejaJouee = true;
            }
        
            // if(dejaJouee)
            // {
            //     AnimGong.Marche();
            //     dejaJouee = false;
            // }

            pathFinder.acceleration = 8;
            pathFinder.stoppingDistance = 0;
            
            StartCoroutine(GongWave());
        }
        
        if (currentState == State.Patrolling)
        {
            //marche lentee
            if(dejaJouee)
            {
                //AnimGong.Marche();
                //dejaJouee=false;
            }
            

            pathFinder.acceleration = 1;
            pathFinder.stoppingDistance = 0;
            if (!pathFinder.pathPending && pathFinder.remainingDistance < 0.5f)
                GotoNextPoint();
        }
    }

    IEnumerator GongWave()
    {
        if (Time.time > nextGongTime && !stunned)
        {
            if(SoundActiveGong==false)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/Cloches/Boss_cloche",transform.position);
                SoundActiveGong=true;
            }
            nextGongTime = Time.time + gongTimer;
            pathFinder.enabled = false;
            gongWaveAnimator.SetTrigger("Elargi");
            //Invoke("desactiveOnde", 3f);
            pathFinder.enabled = true;
            AnimGong.GongActiver();   
        }
        yield return null;
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
    
        //"Animation" de Lunge
        AnimGong.animCac();

        while (percent <= 1)
        {
            if (percent >= .5f && !appliedDamage)
            {
                appliedDamage = true;
                targetVie.TakeDamage(damage);
            }

            transform.LookAt(_hunter.transform.position);
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;
        }
        
        //currentState = State.Gonging;
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
        FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/DestructionGONG", transform.position);


        
        //Animation de mort
        
        
        Destroy(
            Instantiate(DeathEffect, transform.position,
                Quaternion.FromToRotation(Vector3.forward, transform.position)),
            DeathEffect.main.startLifetimeMultiplier);
    }
    
    private void desactiveOnde()
    {
        gongWaveAnimator.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        Lance lance = other.gameObject.GetComponent<Lance>();

        if (lance != null)
        {
            stunned = true;
            StartCoroutine(StunXseconds(5f));
        }
    }
}
