using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    float lookAheadDistance = 2;

    [SerializeField] float _slerpStepFactor = .1f;
    [SerializeField] float redirectThreshold = 2f;
    [SerializeField] float forwardSpeed = .2f;
    [SerializeField] bool useDummyHelper = false;

    [Tooltip("Randomizing the distance to the new destination")]
    [SerializeField] Vector2 lookAheadRange = new Vector2(4, 8);

    [Tooltip("How far to check if too far from destination")]
    [SerializeField] float distanceLimit = 5f;
    private Vector3 destination;
    private Vector3 direction;
    //private Vector3 initialPos;
    //private Quaternion initialRot;
    private float maxX;
    private float minX;
    private float maxZ;
    private float minZ;
    private float maxY;
    [HideInInspector] public float minY;
    public enum moveState { WANDER, GOT_HIT, DEAD }
    public moveState currentState = moveState.WANDER;
    private LaserLine[] lasers;
    //private int health = 100;
    private Transform dummyTransform;

    void Start()
    {
        lasers = GetComponentsInChildren<LaserLine>();

        if (useDummyHelper)
            dummyTransform = GameObject.Find("DummyHelper").GetComponent<Transform>();

        destination = transform.position + Vector3.forward;
        //initialPos = transform.position;
        //initialRot = transform.rotation;

        // Helper dummies located in scene to visualize boundary
        maxX = GameObject.Find("MaxX").GetComponent<Transform>().position.x;
        minX = GameObject.Find("MinX").GetComponent<Transform>().position.x;
        minZ = GameObject.Find("MinZ").GetComponent<Transform>().position.z;
        maxZ = GameObject.Find("MaxZ").GetComponent<Transform>().position.z;
        minY = GameObject.Find("MinY").GetComponent<Transform>().position.y;
        maxY = GameObject.Find("MaxY").GetComponent<Transform>().position.y;

    }
    /*[SerializeField] Vector2 laserXRange = new Vector2(-1.5f, 1.5f);
    [SerializeField] Vector2 laserYRange = new Vector2(-1.5f, 1.5f);
    [SerializeField] float  zVal = -2.0f;
    [SerializeField] float originOffset;*/
    /*void setLaserRange()
    {
        foreach(LaserLine l in lasers)
        {
            l.xRange = laserXRange;
            l.yRange = laserYRange;
            l.zValue = zVal;
            l.originOffset = originOffset;
        }
    }*/

    // Update is called once per frame
    float dist;
    float _slerpStep;
    void Update()
    {
        
        switch (currentState)
        {
            case moveState.WANDER:
                dist = Vector3.Distance(this.transform.position, destination);

                if (dist < redirectThreshold)
                    makeNewDestination();



                for (int i = 0; i < 10; i++)
                {
                    if (!pathIsBlocked())
                        break;
                    makeNewDestination();

                    if (i == 9)  // The drone escaped the bounds, bring it back to right over the yard
                        destination = new Vector3(-.15f, 6.84f, -12.49f);
                }


                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), _slerpStep);


                this.transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * forwardSpeed);  //.Translate(Vector3.forward + Time.deltaTime * forwardSpeed);
                break;

            case moveState.GOT_HIT:



                break;
        }
    }
    void makeNewDestination()
    {
        lookAheadDistance = Random.Range(lookAheadRange.x, lookAheadRange.y);
        float x = Random.Range(-1f, 1f);  // only a 45 degree range of verticle randomness
        float y = Random.Range(-.2f, .2f); // 360 degrees of  randomness
        float z = Random.Range(-1f, 1f );
        //print("min phi: " + (-Mathf.PI / 16 - Mathf.PI / 2) * (180 / Mathf.PI));
        //print("max phi: " + (Mathf.PI / 16 - Mathf.PI / 2) * (180 / Mathf.PI));
        
        //Vector3 _pos = this.transform.position;
        direction = new Vector3(x, y, z);
        destination = transform.position + direction * lookAheadDistance;

        // If the new destination is farther away, rotate slower. In Psuedocode: dist[ance].isInverseTo(slerpStep)
        _slerpStep = _slerpStepFactor / dist;

        //print("makeNewDestionation() called");

        if(useDummyHelper)
            dummyTransform.position = destination;

        
    }
    private bool pathIsBlocked()
    {
        // Check if inside outer boundary
        if (destination.x < minX || destination.x > maxX || destination.z < minZ || destination.z > maxZ || destination.y < minY || destination.y > maxY)
        {
            //transform.position = new Vector3(0, 0, 0);
            return true;
        }

        //TODO: Since boundary is square, it would be a lot more optimized just to do a range check for the destination, until more obstacles are added
        Ray r = new Ray(this.transform.position, direction);
        RaycastHit[] hits = Physics.RaycastAll(r, lookAheadDistance , mask);

        
        return hits.Length != 0;
    }

    

    /*public void takeDamage(int damage)
    {
        health -= damage;
    }*/
}
