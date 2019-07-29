using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    [SerializeField] float hitForceFactor = 15f;
    [SerializeField] float recoverTime = .25f;

    public float health = 100f;
    private Rigidbody rb;
    private DroneMovement dm;
    private Animator animator;
    public bool isAlive = true;

    private GameObject laserGroup;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        dm = GetComponent<DroneMovement>();
        animator = GetComponent<Animator>();
        //laserGroup = transform.GetChild(3).GetChild(0).gameObject;
        
    }

    public void takeDamage(float damage, RaycastHit hit, Ray r, LayerMask m)
    {
        if (!isAlive)
            return;

        health -= damage;

        if (health < .000001f)
            die();

        if (!isAlive)
            return;

        dm.currentState = DroneMovement.moveState.GOT_HIT;
        rb.isKinematic = false;
        rb.AddForceAtPosition((r.direction.normalized - hit.normal) * (hitForceFactor * ((100 - health)/5)) , hit.point);
        Invoke("recoverFromHit", recoverTime);

       
        

        DamageLogicForShader[] _items = hit.transform.GetComponentsInChildren<DamageLogicForShader>(false);

        foreach (DamageLogicForShader _item in _items)
        {
            _item.updateShaderDamage(new Vector4(hit.textureCoord.x, hit.textureCoord.y));
        }

    }

  

  

    private void die()
    {
        // TODO: make the red light go off

        //gameObject.layer = 9;  //This is so you can still shoot the drone around when it dies.
        //hitForceFactor = 200f;
        isAlive = false;
        Destroy(rb); // don't want to interfere with the soon to be activated child rigid bodies
        dm.enabled = false;
        animator.enabled = false;
        //laserGroup.SetActive(false);
        dm.currentState = DroneMovement.moveState.DEAD;
        transform.GetChild(3).gameObject.SetActive(false);// turn off the regular model
        transform.GetChild(4).gameObject.SetActive(true);// turn on the fractured model and explode via OnEnable in Blowup script
        GetComponent<AudioSource>().Play();
        Invoke("destroyDrone", 10f);
        //concaveCollider.enabled = false;

    }
    private void destroyDrone()
    {
        Destroy(gameObject);
    }
    private void recoverFromHit()
    {
        

        dm.currentState = DroneMovement.moveState.WANDER;
        rb.isKinematic = true;
    }
}
