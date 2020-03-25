using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Ennemi_2 : Vivant
{
    /*[SerializeField] private int speed = 12;
    [SerializeField] private GameObject Joueur;
    [SerializeField] private GameObject _mainDroite;
    [SerializeField] private GameObject _missile;
    [SerializeField] private bool AssezProche=false;
    public ParticleSystem DeathEffect;

    private float distance;
    public float _distanceNecessaire;
    private Vivant PlayerHealth;
    private int _missileMax=10;
    [SerializeField] private int _missileTire = 0;
    private float _tempsTire = 0f;
    [SerializeField]private float timer = 0f;

    private void Start()
    {
        Joueur=GameObject.Find("Player");
    }
    void Update()
    {
        Debug.DrawLine(this.transform.position, Joueur.transform.position, Color.black);

        distance = Joueur.transform.position.magnitude - this.transform.position.magnitude;

        if (distance < _distanceNecessaire)
        {
            AssezProche = true;
        }

        else
        {
            AssezProche = false;
        }

        if (AssezProche == true)
        {
            if (Joueur != null)
            {
                this.transform.LookAt(Joueur.transform);
            }

            timer += Time.deltaTime;
            if (timer > 2f && _missileTire < _missileMax)
            {
                _tempsTire += Time.deltaTime;
                if (_tempsTire > 0.5f)
                {
                    Tire(_mainDroite);
                    _tempsTire = 0f;
                }


            }

            if (_missileTire == _missileMax)
            {
                timer = 0f;
                _missileTire = 0;
            }
        }
    }

    void FixedUpdate()
    {
        if (Joueur != null)
        {
            Focalisation(Joueur);
        }
    }

    private void Focalisation(GameObject J)
    {
        Vector3 direction = J.transform.position - this.transform.position;
        if (direction.magnitude > 10)
        {
            this.GetComponent<NavMeshAgent>().SetDestination(J.gameObject.transform.position);
        }
        else
        {
            this.GetComponent<NavMeshAgent>().SetDestination(this.transform.position);
        }
    }

    private void Tire(GameObject Main)
    {
        GameObject Missile = Instantiate(_missile);
        Missile.transform.position = Main.transform.position;
        Missile.transform.rotation = this.transform.rotation;
        _missileTire++;
        Destroy(Missile,10f);
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Lance"))
        {
            Destroy(gameObject);
            Destroy(Instantiate(DeathEffect, transform.position, Quaternion.FromToRotation(Vector3.forward, transform.position)), DeathEffect.main.startLifetimeMultiplier);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Lance"))
        {

            TakeDamage(5f);
            if (health <= 0)
            {
                Destroy(gameObject);
                Destroy(
                    Instantiate(DeathEffect, transform.position,
                        Quaternion.FromToRotation(Vector3.forward, transform.position)),
                    DeathEffect.main.startLifetimeMultiplier);
            }
        }
    }
    */

    


}
