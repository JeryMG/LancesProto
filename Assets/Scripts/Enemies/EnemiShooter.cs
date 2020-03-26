using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemiShooter : Vivant
{
    public GameObject prefab;
    public Transform outputTransform;
    public Transform Container;

    [SerializeField] private float FireRate = 0.3f;
    private float nextAttackTime;

    private Transform target;
    private bool hasTarget;
    private NavMeshAgent pathFinder;
    [SerializeField] private float idleDistanceTreshold = 10f;

    protected override void Start()
    {
        base.Start();
        target = FindObjectOfType<Hunter>().transform;
        if (target != null)
        {
            hasTarget = true;
        }
        pathFinder = GetComponent<NavMeshAgent>();
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
        if (sqrDstToTarget < Mathf.Pow(idleDistanceTreshold, 2))
        {
            //son tir
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/Tir", transform.position);


            Tir();
        }
        
        if (this.health <= 0)
        {
            //son de destruction 
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/DestructionEnnemi",transform.position);
            
            Destroy(gameObject);
        }
    }
    
    public IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while (hasTarget)
        {
            pathFinder.enabled = true;
            Vector3 targetPosition = new Vector3(target.position.x, 0 , target.position.z);
            if (!dead) 
            { 
                pathFinder.SetDestination(targetPosition);
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    private void Tir()
    {
        if (Time.time > nextAttackTime && target != null)
        {
            transform.LookAt(target);
            nextAttackTime = Time.time + FireRate;
            GameObject newProjectile =
                Instantiate(prefab, outputTransform.position, outputTransform.rotation, Container);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        bool hitted = false;
        Lance newLance = other.gameObject.GetComponent<Lance>();
        if (newLance != null && !hitted)
        {
            TakeDamage(newLance.lanceDamage);
            hitted = true;
        }
    }
}
