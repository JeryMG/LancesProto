using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _missileEnnemi : MonoBehaviour
{
    private bool fk = false;
    void Update()
    {
        this.transform.position+=this.transform.forward*Time.deltaTime*30f;
      
    }


    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Mur");

        

        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Vivant>().TakeDamage(2f);
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            Debug.Log("Thomas");
            Destroy(this.gameObject);
        }
    }

}
