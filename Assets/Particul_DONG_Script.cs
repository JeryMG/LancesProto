using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particul_DONG_Script : MonoBehaviour
{
    public Vector3 speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(speed * Time.deltaTime);
    }
}
