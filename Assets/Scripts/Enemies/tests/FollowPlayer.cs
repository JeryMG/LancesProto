using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FollowPlayer : MonoBehaviour
{
    private NavMeshAgent pathfinder;
    private GameObject _player;
    private Vivant entity;
    private Rigidbody rb;
    
    void Start()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        entity = GetComponent<Vivant>();
        rb = GetComponent<Rigidbody>();
        
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        StartCoroutine(resfreshDestination());
    }

    IEnumerator resfreshDestination()
    {
        if (!entity.stunned)
        {
            rb.velocity = Vector3.zero;
            pathfinder.enabled = true;
            float refreshRate = 0.25f;
            pathfinder.SetDestination(_player.transform.position);

            yield return new WaitForSeconds(refreshRate);
        }
        else
        {
            pathfinder.enabled = false;
        }
    }
}
