using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoteurProjectile : MonoBehaviour
{
    [SerializeField] private float forceProjectile = 10f;
    private Rigidbody rb;
    public float Lifetime = 3f;
    [SerializeField] private float damage = 10f;

    public bool parried = false;
    private Hunter playerPos;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, Lifetime);
        playerPos = FindObjectOfType<Hunter>();
    }

    // Update is called once per frame
    void Update()
    {
        go();
    }

    public void go()
    {
        Vector3 direction = new Vector3();
        
        if (parried)
        {
            direction = transform.parent.parent.position - transform.position;
        }
        else
        {
            direction = playerPos.transform.position - transform.position;
        }
        direction.y = transform.parent.position.y;
        
        rb.AddForce(/*transform.parent.parent.forward*/ direction.normalized * forceProjectile, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        Vivant Entity = other.transform.GetComponent<Vivant>();

        if (Entity != null)
        {
            Entity.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
