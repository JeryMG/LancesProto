using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimGong : MonoBehaviour
{
    public Animator anim;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)&&anim.isActiveAndEnabled==true)
        {

            anim.enabled = true;
            anim.Play("Corps", 0,0f);
            anim.Play("Arme", 1,0f);
            anim.Play("Armure", 2,0f);
        }
        
    }
}
