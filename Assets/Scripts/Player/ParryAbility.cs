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
                //projectile.parried = true;
                Debug.Log("Parried !!!!!!!");
                StartCoroutine(changeColor());
                Destroy(other.gameObject);
            }
        }
    }
    
    IEnumerator changeColor()
    {
        skinMat.color = Color.magenta;
        yield return new WaitForSeconds(1f);
        skinMat.color = playerColor;
    }
}
