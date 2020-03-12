using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Lance : MonoBehaviour
{
    [SerializeField]private float lanceSpeed = 12f;
    public bool stop;
    private Rigidbody lanceBody;
    private Hunter _player;
    public float lanceDamage = 5f;

    private void Start()
    {
        lanceBody = GetComponent<Rigidbody>();
        StayImmobile(true);
        stop = true;
        _player = FindObjectOfType<Hunter>();
    }

    private void Update()
    {
        if (!stop)
        {
            shooting();
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.tag!= "Player" && !_player.aiming)
        {
            lanceBody.velocity = Vector3.zero;
            stop = true;
            StayImmobile(true);
            Debug.Log("cible ajoutée");
            if (!_player.lieuxDeTp.Contains(transform.position))
            {
                _player.lieuxDeTp.Add(transform.position);
            }
        }

        Enemi enemiBody = other.gameObject.GetComponent<Enemi>();
        if (enemiBody != null)
        {
            enemiBody.TakeDamage(lanceDamage);
        }
    }

    public void shooting()
    {
        transform.Translate(Vector3.forward * lanceSpeed * Time.deltaTime);
    }

    public void StayImmobile(bool yes)
    {
        if (yes)
        {
            lanceBody.useGravity = false;
            lanceBody.isKinematic = true;
        }
        if (!yes)
        {
            lanceBody.useGravity = true;
            lanceBody.isKinematic = false;
        }
    }
}
