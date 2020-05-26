using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cone_Anim : MonoBehaviour
{
    [HideInInspector] public bool jouer=false;
    public Animator Anim;

    private void Update() {
        if(jouer==true&&Anim.GetCurrentAnimatorStateInfo(0).normalizedTime>1f)
        {   
            jouer=false;
        }
    }
    public void Gong()
    {   if(jouer==false)
        {
            Anim.Play("AnimConeDong",0,0f);     
            jouer=true;
        }  
    }
}
