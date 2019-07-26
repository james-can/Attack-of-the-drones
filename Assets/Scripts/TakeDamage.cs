using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    [SerializeField] float hitForceFactor = 15f;
    [SerializeField] float recoverTime = .25f;

    private float health = 100f;
    private Rigidbody rb;
    private DroneMovement dm;
    private Animator animator;
    public bool isAlive = true;
   

    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        dm = GetComponent<DroneMovement>();
        animator = GetComponent<Animator>();
        
        
    }

    public void takeDamage(float damage, RaycastHit hit, Ray r, LayerMask m)
    {
        health -= damage;

        if (health < .000001f)
            die();

        dm.currentState = DroneMovement.moveState.GOT_HIT;
        rb.isKinematic = false;
        rb.AddForceAtPosition((r.direction.normalized - hit.normal) * hitForceFactor, hit.point);
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

        gameObject.layer = 9;  //This is so you can still shoot the drone around when it dies.
        hitForceFactor = 200f;
        isAlive = false;
        rb.useGravity = true;
        dm.enabled = false;
        animator.enabled = false;
        //concaveCollider.enabled = false;
       
    }
    private void recoverFromHit()
    {
        if (!isAlive)
            return;

        dm.currentState = DroneMovement.moveState.WANDER;
        rb.isKinematic = true;
    }
}
