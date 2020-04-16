 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimHeros : MonoBehaviour
{
    public Animator anim;
    

    public  void Update() {
        if(Input.GetMouseButtonDown(0))
        {
            AnimLancer();
        }
    }
    public void AnimPause()
    {               
        ResetLayerH();
        anim.Play("PauseCorps", 0,0f);
    }
    public void AnimCourse()
    {
        ResetLayerH();
        anim.SetLayerWeight(1,1);
        anim.Play("CCorps",1,0f);
    }
    public void AnimLancer()
    {
        ResetLayerH();
        anim.SetLayerWeight(2,1);
        anim.SetLayerWeight(3,1);
        anim.Play("LLance", 2,0f);
        anim.Play("LCorps", 3,0f);
    }
    public void AnimCac()
    {
        ResetLayerH();
        anim.SetLayerWeight(5,1);
        anim.SetLayerWeight(4,1);
        anim.Play("CacLance", 5,0f);
        anim.Play("CacCorps", 4,0f);
    }
    public  void ResetLayerH()
    {
        for (int i = 1; i < anim.layerCount; i++)
            {
                anim.SetLayerWeight(i,0);
            }
    }
}
