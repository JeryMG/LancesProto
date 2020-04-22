using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _E_Gong_Cac : MonoBehaviour
{

    public E_Gong fdd ;
    public GameObject joueur;
    private void Start() 
    {
        joueur=this.gameObject.GetComponentInParent<GameObject>();
        fdd=GetComponentInParent<E_Gong>();
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag=="Player")
        {
            fdd.JoueurAuCac=true;
            joueur.GetComponent<E_Gong>().JoueurAuCac=fdd.JoueurAuCac;
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag=="Player")
        {
            fdd.JoueurAuCac=false;
            joueur.GetComponent<E_Gong>().JoueurAuCac=fdd.JoueurAuCac;
        }
    }
}
