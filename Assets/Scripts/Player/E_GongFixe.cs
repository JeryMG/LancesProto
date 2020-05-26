using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_GongFixe : Vivant
{
    public enum State
    {
        //quand le joueur est mort
        Idle,
        //quand le joueur est dans sa range de gong mais hors de sange cac
        Gonging,
        //quand le joueur se trouve tres proche (range de cac)
        Attacking,
    }
    
    public State currentState;
    private bool hasTarget;
    private Hunter _hunter;
    private Transform target;
    private Vivant targetVie;
    public GameObject HPpack;
    
    [Header("Mode Idle")]
    [SerializeField] private float idleDistanceTreshold = 40f;
    
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
    private int RandAnimCac=1;
    private bool SoundActiveGong=false;
    public GameObject Gauche;
    public GameObject Droite;
    

    //[SerializeField] private List<AnimatorController> Anim =new List<AnimatorController>();
    //public Animator animPerso;

    protected override void Start()
    {
        base.Start();
        AnimGong = GetComponentInChildren<TestAnimGong>();
        
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Idle;
            hasTarget = true;
            _hunter = FindObjectOfType<Hunter>();
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetVie = _hunter.GetComponent<Vivant>();

            //StartCoroutine(UpdatePath());
        }
        OnDeath += enemyDeath;
        OnDeath += spawnPV;
    }

    private void Update()
    {
        
        if(target!=null)
        {
            float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
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
                 dejaJouee=false;
            }

            if (stunned)
            {
                currentState = State.Idle;
            }
            
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
            //anim repos
        }
        
        if (currentState == State.Gonging)
        {
            if (dejaJouee==true)
            {
                if(AnimGong.anim.GetCurrentAnimatorStateInfo(5).normalizedTime>0.25&&AnimGong.anim.GetCurrentAnimatorStateInfo(5).normalizedTime<0.35)
                {
                    Vector3 _tetePos=this.transform.position;
                    _tetePos.y+=4;
                    Destroy(
                Instantiate(Gauche, _tetePos,
                Quaternion.FromToRotation(Vector3.forward, transform.position)),
                1);
                Destroy(
                Instantiate(Droite, _tetePos,
                Quaternion.FromToRotation(Vector3.forward, transform.position)),
                1);
                }
                if(AnimGong.anim.GetCurrentAnimatorStateInfo(5).normalizedTime>0.35)
                {
                    if(SoundActiveGong==false)
                    {
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/Cloches/Boss_cloche",transform.position);
                        SoundActiveGong=true;
                    }
                }
            }
            StartCoroutine(GongWave());
        }
    }

    [ContextMenu("propage")]
    IEnumerator GongWave()
    {
        if (Time.time > nextGongTime && !stunned)
        {
            if(dejaJouee==false)
            {
                dejaJouee=true;
            }
            
            nextGongTime = Time.time + gongTimer;
            gongWaveAnimator.SetTrigger("Elargi");
            //Invoke("desactiveOnde", 3f);
            AnimGong.GongActiver();
        }
        yield return null;
    }
    
    IEnumerator Attack()
    {
        currentState = State.Attacking;

        Vector3 originalPosition = transform.position;
        Vector3 attackPosition = target.position - (target.position - transform.position).normalized;
        float attackSpeed = 3f;
        float percent = 0;
        bool appliedDamage = false;
    
        //"Animation" de Lunge
        AnimGong.animCac();

        while (percent <= 1)
        {
            StopCoroutine(GongWave());
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
    }
    
    /*void GotoNextPoint() 
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
    }*/
    
    void enemyDeath()
    {
        //son de destruction 
        FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/DestructionGONG", transform.position);
        
        //Animation de mort

        Destroy(
            Instantiate(DeathEffect, transform.position,
                Quaternion.FromToRotation(Vector3.forward, transform.position)),
            DeathEffect.main.startLifetimeMultiplier);
        spawnPV();
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

    private void spawnPV()
    {
        Instantiate(HPpack, transform.position, Quaternion.identity);
    }
}
