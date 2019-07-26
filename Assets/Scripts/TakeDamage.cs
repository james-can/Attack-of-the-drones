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
    private bool isAlive = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        dm = GetComponent<DroneMovement>();
        animator = GetComponent<Animator>();
    }

    public void takeDamage(float damage, RaycastHit hit, Vector3 shootingDirection)
    {
        health -= damage;

        if (health < .000001f)
            die();

        dm.currentState = DroneMovement.moveState.GOT_HIT;
        rb.isKinematic = false;
        rb.AddForceAtPosition((shootingDirection - hit.normal) * hitForceFactor, hit.point);
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
        hitForceFactor = 200f;
        isAlive = false;
        rb.useGravity = true;
        dm.enabled = false;
        animator.enabled = false;
       
    }
    private void recoverFromHit()
    {
        if (!isAlive)
            return;

        dm.currentState = DroneMovement.moveState.WANDER;
        rb.isKinematic = true;
    }
}
