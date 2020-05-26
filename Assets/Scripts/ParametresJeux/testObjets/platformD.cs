using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformD : MonoBehaviour
{
    public float fadeTime;
    public Animator animator;
    private MeshCollider meshCollider;

    private void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(disappearX(fadeTime));
        }
    }

    IEnumerator disappearX(float seconds)
    {
        Vector3 pos = transform.position;
        yield return new WaitForSeconds(seconds);
        animator.SetBool("Drop", true);
        yield return new WaitForSeconds(1.3f);
        transform.parent.gameObject.SetActive(false);
        animator.SetBool("Drop", false);
        transform.position = pos;
        meshCollider.convex = false;
        meshCollider.isTrigger = false;

    }
}
