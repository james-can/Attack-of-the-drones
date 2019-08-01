using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowUp : MonoBehaviour
{
    [SerializeField] float radius = 5;
    [SerializeField] Vector2 forceRange = new Vector2(250, 1000);
    [SerializeField] GameObject explosionParticles;
    private void OnEnable()
    {

        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        foreach(Transform t in transform)
        {
            t.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position, 5f);
        }
    }
}
