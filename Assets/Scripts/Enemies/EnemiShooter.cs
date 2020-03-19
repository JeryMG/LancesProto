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

    private Transform playerTransform;
    [SerializeField] private float idleDistanceTreshold = 10f;

    protected override void Start()
    {
        base.Start();
        playerTransform = FindObjectOfType<Hunter>().transform;
    }

    private void Update()
    {
        float sqrDstToTarget = (playerTransform.position - transform.position).sqrMagnitude;
        if (sqrDstToTarget < Mathf.Pow(idleDistanceTreshold, 2))
        {
            Tir();
        }
        
        if (this.health <= 0)
        {
            //son de destruction 
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/DestructionEnnemi",transform.position);
            
            Destroy(gameObject);
        }
    }

    private void Tir()
    {
        if (Time.time > nextAttackTime && playerTransform != null)
        {
            transform.LookAt(playerTransform);
            nextAttackTime = Time.time + FireRate;
            GameObject newProjectile =
                Instantiate(prefab, outputTransform.position, outputTransform.rotation, Container);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Lance newLance = other.gameObject.GetComponent<Lance>();
        if (newLance != null)
        {
            TakeDamage(newLance.lanceDamage);
        }
    }
}
