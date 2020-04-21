using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawPlat : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).gameObject.activeSelf)
            {
                StartCoroutine(reappearX(2, i));
            }
        }
    }
    
    IEnumerator reappearX(float seconds, int number)
    {
        yield return new WaitForSeconds(seconds);
        transform.GetChild(number).gameObject.SetActive(true);
    }
}
