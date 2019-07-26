using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class ViveInput : MonoBehaviour
{
    
    public SteamVR_ActionSet shootingActionSet;
    public GameObject shellPrefab;
    public float speed = 2;
    public SteamVR_Input_Sources source;
    public float vibrationDuration = 2;
    public float vibrationAmplitute = 5;
    public float vibrationFrequency = 3;
    
    public LayerMask mask; // "Enemy" is name of the layer used here
    private Transform shellDirectionVector;
    private AudioSource shotgunBlast;
    private Transform shellOrigin;
    private Transform rayOrigin;
    private MaterialPropertyBlock propBlock;
    private Renderer _renderer;


    [SerializeField] GameObject sparkParticlePrefab;
    [SerializeField] float hitDamage = 35f;
    [SerializeField] bool useMouseForTesting = false;
    [SerializeField] new Camera camera;
    [SerializeField] Material droneMaterial;

    public delegate void GunFireAction();
    public static event GunFireAction OnGunFired;

    public delegate void RaycastCompletedAction();
    public static event RaycastCompletedAction OnRaycastCompleted;

    // Start is called before the first frame update
    void Start()
    {
        Valve.VR.InteractionSystem.Teleport.instance.CancelTeleportHint();
        shotgunBlast = GameObject.Find("GunRelated/Gun").GetComponent<AudioSource>();
        shellDirectionVector = GameObject.Find("ShellDirectionVector").GetComponent<Transform>();
        shellOrigin = GameObject.Find("ShellOrigination").GetComponent<Transform>();
        rayOrigin = GameObject.Find("GunRelated/Gun").GetComponent<Transform>();

        propBlock = new MaterialPropertyBlock();

    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 touch = SteamVR_Input.GetVector2Action("joystick_touchpad").axis;


        //print("Trigger Axis: " + SteamVR_Input.GetAction<SteamVR_Action_Single>("triggerpullanimate").axis);


        //print("touch x : " + touch.x);
        //print("touch y: " + touch.y);
        

        if (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("fire").stateDown || (useMouseForTesting && Input.GetMouseButtonDown(0)))
        {
            
            
            Rigidbody rb = Instantiate<Rigidbody>(shellPrefab.GetComponent<Rigidbody>(), shellOrigin.position, shellOrigin.rotation);

            // If this were attached to the gun itself, could use Vector3.right here instead, it'd be a helluvalot simpler
            rb.velocity =  Vector3.Normalize(shellDirectionVector.position - shellOrigin.position); 

            if(!useMouseForTesting)
                SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic").Execute(0, vibrationDuration, vibrationFrequency, vibrationAmplitute, source);

            shotgunBlast.Play();
            
            
           

            RaycastHit hit = new RaycastHit();
            
            bool is2dHit = useMouseForTesting && Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 300f, mask); // click to fire at the drones on desktop
            Ray r = new Ray(rayOrigin.position, rayOrigin.forward);
            if (is2dHit || Physics.Raycast(r, out hit, Mathf.Infinity, mask))
            {
                TakeDamage td = hit.transform.GetComponent<TakeDamage>();


               


                

               

                Instantiate(sparkParticlePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                hit.transform.gameObject.GetComponents<AudioSource>()[0].Play();
                td.takeDamage(hitDamage, hit, r, mask);

                

                /*print("textureCoord.x: " + hit.textureCoord.x);
                print("textureCoord.y: " + hit.textureCoord.y);
                print("hit object name: " + hit.transform.name);*/



            }
        
        }
    }

    public void OnObjectPickedUp()
    {
        
        print("OnObjectPickedup called");
       
        shootingActionSet.Activate();
    }

    public void OnObjectDropped()
    {
        print("OnObjectDropped called");
        shootingActionSet.Deactivate();
        //defaultActionSet.Activate();
    }
}
