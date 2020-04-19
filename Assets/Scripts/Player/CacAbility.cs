using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Hunter), typeof(PlayerInputs))]
public class CacAbility : MonoBehaviour
{
    private Hunter _player;
    private PlayerInputs playerInputs;
    private bool inputPressed;

    [Header("KaK Variables")] 
    [SerializeField] float KakDamage = 2f;
    public Transform hitBox;
    [SerializeField] private Vector3 hitBoxSize = new Vector3(3, 1.5f, 2);
    [SerializeField] private float kakForce = 2f;

    private void Start()
    {
        _player = GetComponent<Hunter>();
        playerInputs = GetComponent<PlayerInputs>();
    }

    private void Update()
    {
        if (playerInputs.Melee && _player.currentState == Hunter.states.blinker)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Joueur3D/CAC_Swift", transform.position);
                
            //CacEffect=GameObject.Instantiate(ff,this.transform.position,Quaternion.Euler(new Vector3(-90,this.transform.eulerAngles.y,0)));
            

            Collider[] _hitbox = Physics.OverlapBox(hitBox.position, hitBoxSize / 2);
            foreach (Collider hits in _hitbox)
            {
                if (!hits.CompareTag("Player"))
                {
                    Vivant hitable = hits.GetComponent<Vivant>();
                    Rigidbody hitBody = hits.GetComponent<Rigidbody>();

                    if (hitable != null)
                    {
                        //Son CAC quand il touche
                        RuntimeManager.PlayOneShot("event:/Event3D/Joueur3D/CAC", transform.position);

                        Debug.Log("hitted");
                        hitable.stunned = true;
                        hitable.StartCoroutine(hitable.StunXseconds(1));
                        hitable.TakeDamage(KakDamage);
                        
                        Vector3 direction = hitable.transform.position - transform.position;
                        direction = direction.normalized * kakForce;
                        hitBody.AddForce(direction, ForceMode.Impulse);
                        //lerp pos y
                    }
                }
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(hitBox.position, hitBoxSize);
    }
}
