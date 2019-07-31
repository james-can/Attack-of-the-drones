using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLine : MonoBehaviour
{
    LineRenderer line;
    [SerializeField] float laserRange = 100;
    [SerializeField] float lazerAnimateSpeed = 2;
    [SerializeField] public Vector2 xRange;
    [SerializeField] public Vector2 yRange;
    
    [SerializeField]  float originOffset = .1f;
    [SerializeField] float redirectThreshold = .5f;
    [SerializeField] float distanceAdjustmentFactor;
    float minY;

    [Tooltip("How much more deviation in the rotation of the lasers")]

    public Transform rootTransform;
    private Vector3 destinationPoint;
    private Vector3 rayDirection;
 
    //private Transform parentTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        setDestination();
        line = GetComponent<LineRenderer>();
        minY = GameObject.Find("DroneBoundary/MinY").GetComponent<Transform>().position.y;
        //parentTransform = GetComponentInParent<Tra>
        InvokeRepeating("checkForNewDestination", 1.0f, .1f);
    }

    // Update is called once per frame
    void Update()
    {
        float dist = checkForNewDestination();

        line.SetPosition(0, rootTransform.position + rootTransform.forward * originOffset);
        line.SetPosition(1, Vector3.MoveTowards(line.GetPosition(1), destinationPoint, Time.deltaTime * lazerAnimateSpeed * (dist * distanceAdjustmentFactor)));
        
    }

    private float checkForNewDestination() {
        float dist = Vector3.Distance(destinationPoint, line.GetPosition(1));
        if (dist < redirectThreshold)
        {
            setDestination();
        }
        return dist;
    }

    private void setDestination()
    {
        // -.63 is the y position value of the MinY game object. for some reason wasn't able to 
        // programatically retrieve the value
        float relativeHeight = rootTransform.position.y - minY;// -.63f;


        float x = Random.Range(xRange.x, xRange.y); 
        float y = Random.Range(yRange.x, yRange.y);


        rayDirection =  Vector3.Normalize((rootTransform.forward + new Vector3(x / ( 2 * relativeHeight), y  / (2 * relativeHeight), 0)));
        destinationPoint = rootTransform.position + rayDirection * laserRange;
        
    }
    

}
