using System;
using System.Collections;
using System.Collections.Generic;
//using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FollowPlayer : MonoBehaviour
{
    private NavMeshAgent pathfinder;
    private GameObject _player;
    private Vivant entity;
    private Rigidbody rb;
    private float distance;
    public  Vector3 direction;
    private EnemiShooter shooter;
    
    //raycast
    public Transform target;
    private Vector3 pointToReach;
    private NavMeshHit hit;
    private bool blocked = false;
    
    void Start()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        entity = GetComponent<Vivant>();
        rb = GetComponent<Rigidbody>();
        shooter = GetComponent<EnemiShooter>();
        
        _player = GameObject.FindGameObjectWithTag("Player");
        target = _player.transform;
    }

    private void Update()
    {
        if (target != null && !entity.dead)
        {
            distance = Vector3.Distance(target.position, transform.position);
            direction = target.position - transform.position;
            direction.y = 0;
            if (shooter != null)
            {
                if (distance < shooter.stopingD)
                {
                    Vector3 newTarget = transform.position - direction * distance;
                    pointToReach = newTarget;
                    pathfinder.stoppingDistance = shooter.stopingD;
                    shooter.facing = false;
                }
                else
                {
                    pathfinder.stoppingDistance = shooter.stopingD;
                    pointToReach = target.position;
                }
            }
            else
            {
                pointToReach=direction;
            }
        
            StartCoroutine(resfreshDestination());
            StartCoroutine(raycastDirection());
        }
    }

    IEnumerator resfreshDestination()
    {
        if (!entity.stunned)
        {
            rb.velocity = Vector3.zero;
            pathfinder.enabled = true;
            float refreshRate = 0.15f;
            if (shooter != null)
            {
                pathfinder.SetDestination(pointToReach);
            }
            else
            {
                pathfinder.SetDestination(target.position);
            }

            yield return new WaitForSeconds(refreshRate);
            //yield return StartCoroutine(TurnToFace(_player.transform.position));
        }
        else
        {
            pathfinder.enabled = false;
        }
    }

    IEnumerator raycastDirection()
    {
        blocked = NavMesh.Raycast(transform.position, pointToReach, out hit, NavMesh.AllAreas);
        Debug.DrawLine(transform.position, pointToReach, blocked ? Color.red : Color.green);
        if (blocked)
            Debug.DrawRay(hit.position, Vector3.up, Color.red);
        yield return null;
    }

    IEnumerator TurnToFace(Vector3 looktarget)
    {
        Debug.Log("YOOOOOO");
        pathfinder.isStopped = true;
        Vector3 dirToLookTarget = (looktarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float turnSpeed = 180;
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
        pathfinder.isStopped = false;
    }
}

