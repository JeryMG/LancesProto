using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_E_Vole : MonoBehaviour
{
    public Animator anim;
    
      private void Start()         
    {
        AnimVole();
    }

    
    public void AnimVoleEchos()
    {               
        ResetLayerE();
        anim.SetLayerWeight(0,1);
        anim.SetLayerWeight(3,1);
        anim.Play("VoleBras",3,0f);
        anim.Play("VoleEchos",0,0f);

    }
    public void AnimVole()
    {
        ResetLayerE();
        anim.SetLayerWeight(2,1);
        anim.SetLayerWeight(3,1);
        anim.Play("Vole",2,0f);
        anim.Play("VoleBras",3,0f);
    }
    public void AnimDegats()
    {
        ResetLayerE();
        anim.SetLayerWeight(1,1);
        anim.Play("DgtsRecus", 1,0f);
    }
    
    public  void ResetLayerE()
    {
        for (int i = 1; i < anim.layerCount; i++)
            {
                anim.SetLayerWeight(i,0);
            }
    }
}
