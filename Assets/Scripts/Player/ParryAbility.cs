using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryAbility : MonoBehaviour
{
    private PlayerInputs playerInputs;
    private Material skinMat;
    private Color playerColor;
    
    void Start()
    {
        playerInputs = FindObjectOfType<PlayerInputs>();
        skinMat= transform.parent.GetComponent<Renderer>().material;
        playerColor = skinMat.color;
    }
    
    private void OnTriggerStay(Collider other)
    {
        MoteurProjectile projectile = other.GetComponent<MoteurProjectile>();
        
        if (projectile != null)
        {
            if (playerInputs.Parry)
            {
                // son Parade
                FMODUnity.RuntimeManager.PlayOneShot("event:/Event2D/Joueur/Parade");

                //projectile.parried = true;
                Debug.Log("Parried !!!!!!!");
                StartCoroutine(changeColor(Color.green));
                Destroy(other.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            print("projectile incoming");
            changeColor(Color.yellow);
        }
    }

    IEnumerator changeColor(Color newColor)
    {
        skinMat.color = newColor;
        yield return new WaitForSeconds(1f);
        skinMat.color = playerColor;
    }
}
