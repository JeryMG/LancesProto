using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GongWave : MonoBehaviour
{
    private Hunter _hunter;
    private Vivant targetVie;
    public GameObject GauchePart;
    public GameObject DroitePart;
    public GameObject Particule;
    float timer=0f;
    bool _ondeTeteJoue=false;
    [SerializeField] private float damage = 2f;
    
    private void Start()
    {
        _hunter = FindObjectOfType<Hunter>();
        targetVie = _hunter.GetComponent<Vivant>();
                Particule.gameObject.SetActive(true);

    }

    private void Update() {
        if(_ondeTeteJoue==true)
        {
            timer+=Time.deltaTime;
            if(timer>=1f)
            {
                _ondeTeteJoue=false;
                timer=0f;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        bool damageTaken = false;
        Debug.Log("hikiki");    
        IClochePropag bro = other.gameObject.GetComponent<IClochePropag>();
        
        if (other.gameObject.CompareTag("Player") && !damageTaken)
        {
            targetVie.TakeDamage(damage);
            damageTaken = true;
        }

            if (other.CompareTag("Enemy")&& CompareTag("WaveOri"))
            {
                Vector3 pos=other.transform.position;
                pos.y+=2f;
                if (bro != null)
                {
                    if(_ondeTeteJoue==false)
                    {
                        Destroy(
                        Instantiate(GauchePart, pos,
                        Quaternion.FromToRotation(Vector3.forward, other.transform.position),other.transform),
                        1);
                        Destroy(
                        Instantiate(DroitePart, pos,
                        Quaternion.FromToRotation(Vector3.forward, other.transform.position),other.transform),
                        1);
                        _ondeTeteJoue=true;
                    }       
                    Debug.Log("lets go !!!!");
                    bro.propagOnde();
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/Cloches/Ennemi_Cloche",transform.position);
                }
            }
    }   
}
