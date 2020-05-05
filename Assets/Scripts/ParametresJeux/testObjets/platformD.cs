using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformD : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(disappearX(2));
        }
    }

    IEnumerator disappearX(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
}
