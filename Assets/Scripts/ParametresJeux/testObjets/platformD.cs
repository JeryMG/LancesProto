using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformD : MonoBehaviour
{
    public float fadeTime;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(disappearX(fadeTime));
        }
    }

    IEnumerator disappearX(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
}
