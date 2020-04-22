using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goproj : MonoBehaviour
{
    public float Speed = 8;
    private ParryAbility _parryAbility;
    private Transform _player;
    private Vector3 direction;
    private float damage = 5f;
   [SerializeField] private float deathTime = 2f;

    private void Start()
    {
        _parryAbility = FindObjectOfType<ParryAbility>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        direction = _player.position - transform.position;
        Destroy(gameObject, deathTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        //transform.position += _player.tran
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vivant player = other.GetComponent<Vivant>();
            _parryAbility.incomings.Remove(this.gameObject);
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
