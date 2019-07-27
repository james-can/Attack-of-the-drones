using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLine : MonoBehaviour
{
    LineRenderer line;
    [SerializeField] float laserRange = 100;
    [SerializeField] float lazerAnimateSpeed = 2;
    [SerializeField] Vector2 xRange;
    [SerializeField] Vector2 yRange;
    [SerializeField] Vector2 zRange;
    [SerializeField] float redirectThreshold = .5f;
    private Vector3 destinationPoint;
    //private Transform parentTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        setDestination();
        line = GetComponent<LineRenderer>();
        //parentTransform = GetComponentInParent<Tra>
        //InvokeRepeating("checkForNewDestination", 1.0f, .3f);
    }

    // Update is called once per frame
    void Update()
    {
        
        line.SetPosition(1, Vector3.MoveTowards(line.GetPosition(1), destinationPoint, Time.deltaTime * lazerAnimateSpeed));
        checkForNewDestination();
    }
    private void setDestination()
    {
        float x = Random.Range(xRange.x, xRange.y); 
        float y = Random.Range(yRange.x, yRange.y);



        destinationPoint = transform.position + Vector3.Normalize((Vector3.forward + new Vector3(x, y, 0))) * laserRange;
        print("destination point: " + destinationPoint);
    }
    private void checkForNewDestination()
    {
        
        if(Vector3.Distance(destinationPoint, line.GetPosition(1)) < redirectThreshold)
        {
            setDestination();
        }
    }

}
