using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimHeros : MonoBehaviour
{
    public Animator anim;
    
    private void Update() {
        if(Input.GetMouseButtonDown(0))
        {           
            
            anim.enabled = true;  
            AnimLancer();
        }
    }

    public void AnimPause()
    {               
        ResetLayer();
        anim.Play("PauseCorps", 0,0f);
    }
    public void AnimCourse()
    {
        ResetLayer();
        anim.SetLayerWeight(1,1);
        anim.Play("CCorps",1,0f);
    }
    public void AnimLancer()
    {
        ResetLayer();
        anim.SetLayerWeight(2,1);
        anim.SetLayerWeight(3,1);
        anim.Play("LLance", 2,0f);
        anim.Play("LCorps", 3,0f);
    }
    public void AnimCac()
    {
        ResetLayer();
        anim.SetLayerWeight(5,1);
        anim.SetLayerWeight(4,1);
        anim.Play("CacLance", 5,0f);
        anim.Play("CacCorps", 4,0f);
    }
    private void ResetLayer()
    {
        for (int i = 1; i < anim.layerCount; i++)
            {
                anim.SetLayerWeight(i,0);
            }
    }
}
