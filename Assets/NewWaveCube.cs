using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWaveCube : MonoBehaviour
{
    private Hunter _hunter;
    private Vivant targetVie;
    public GameObject GauchePart;
    public GameObject DroitePart;

    [SerializeField] private float damage = 2f;
    
    private void Start()
    {
        _hunter = FindObjectOfType<Hunter>();
        targetVie = _hunter.GetComponent<Vivant>();
    }

    private void OnTriggerEnter(Collider other)
    {
        bool damageTaken = false;
        IClochePropag bro = other.gameObject.GetComponent<IClochePropag>();

        if (other.gameObject.CompareTag("Player") && !damageTaken)
        {
            targetVie.TakeDamage(damage);
            damageTaken = true;
        }
        if (other.CompareTag("Enemy")&& CompareTag("WaveOri"))
        {
            if (bro != null)
            {
            Vector3 pos=other.transform.position;
            pos.y+=4f;
                Destroy(
                    Instantiate(GauchePart, pos,
                        Quaternion.FromToRotation(Vector3.forward, other.transform.position),other.transform),
                    1.2f);
                Destroy(
                    Instantiate(DroitePart, pos,
                        Quaternion.FromToRotation(Vector3.forward, other.transform.position),other.transform),
                    1.2f);
                Debug.Log(this.gameObject);
                Debug.Log("lets go !!!!");
                bro.propagOnde();
                FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/Cloches/Ennemi_Cloche",transform.position);
            }
        }
    }
}
