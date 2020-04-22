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
            float refreshRate = 0.15f;
            pathfinder.SetDestination(_player.transform.position);

            yield return new WaitForSeconds(refreshRate);
            //yield return StartCoroutine(TurnToFace(_player.transform.position));
        }
        else
        {
            pathfinder.enabled = false;
        }
    }
    
    IEnumerator TurnToFace(Vector3 looktarget)
    {
        Vector3 dirToLookTarget = (looktarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float turnSpeed = 90;
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

}
