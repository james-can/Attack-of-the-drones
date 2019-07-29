using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowUp : MonoBehaviour
{
    [SerializeField] float radius = 5;
    [SerializeField] float force = 5;
    [SerializeField] GameObject explosionParticles;
    private void OnEnable()
    {

        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        foreach(Transform t in transform)
        {
            t.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius);
        }
    }
}
