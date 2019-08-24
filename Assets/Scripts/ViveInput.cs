using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;


public class ViveInput : MonoBehaviour
{
    
    public SteamVR_ActionSet shootingActionSet;
    public SteamVR_ActionSet defaultActionSet;
    public GameObject shellPrefab;
    public float speed = 2;
    public SteamVR_Input_Sources source;
    public float vibrationDuration = 2;
    public float vibrationAmplitute = 5;
    public float vibrationFrequency = 3;
    
    public LayerMask mask; // "Enemy" is name of the layer used here
    private Transform shellDirectionVector;
    private AudioSource shotgunBlast;
    //private Transform shellOrigin;
    //private Transform rayOrigin;
    private MaterialPropertyBlock propBlock;
    private Renderer _renderer;
    private GameObject selectedGun;
    private Vector3 leftHandGunPos = new Vector3(.56f, -.39f, 1.07f);
    private Vector3 rightHandGunPos = new Vector3(.42f, -.34f, 1.04f);

    [SerializeField] GameObject sparkParticlePrefab;
    [SerializeField] float hitDamage = 35f;
    [SerializeField] bool useMouseForTesting = false;
    [SerializeField] new Camera camera;
    [SerializeField] Material droneMaterial;
    [SerializeField] Hand leftHand;
    [SerializeField] Hand rightHand;
    [SerializeField] Shader highlightShader;
    [SerializeField] Shader standardShader;




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
        //shellOrigin = GameObject.Find("ShellOrigination").GetComponent<Transform>();
        //rayOrigin = GameObject.Find("GunRelated/Gun").GetComponent<Transform>();

        propBlock = new MaterialPropertyBlock();

    }

    // Update is called once per frame
    void Update()
    {
        SteamVR_Action_Boolean dropAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("drop");
        //Vector2 touch = SteamVR_Input.GetVector2Action("joystick_touchpad").axis;
        
        if (dropAction.stateDown)
        {
            if (leftHand.GetDeviceIndex() == dropAction.trackedDeviceIndex)
            {
                leftHand.DetachObject(leftHand.AttachedObjects[0].attachedObject);

            }
            else if (rightHand.GetDeviceIndex() == dropAction.trackedDeviceIndex)
            {
                rightHand.DetachObject(rightHand.AttachedObjects[0].attachedObject);
            }
        }



        //print("Trigger Axis: " + SteamVR_Input.GetAction<SteamVR_Action_Single>("triggerpullanimate").axis);


        //print("touch x : " + touch.x);
        //print("touch y: " + touch.y);
        SteamVR_Action_Boolean grabAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("grabpinch");
        if (grabAction.stateDown)
        {

            if (leftHand.GetDeviceIndex() == grabAction.trackedDeviceIndex)
            {
                Transform modelTransform = leftHand.AttachedObjects[0].attachedObject.transform.GetChild(0);
                modelTransform.localPosition = leftHandGunPos;
                modelTransform.gameObject.GetComponent<Renderer>().material.shader = standardShader;

            }
            else if (rightHand.GetDeviceIndex() == grabAction.trackedDeviceIndex)
            {
                Transform modelTransform = rightHand.AttachedObjects[0].attachedObject.transform.GetChild(0);
                modelTransform.localPosition = rightHandGunPos;
                modelTransform.gameObject.GetComponent<Renderer>().material.shader = standardShader;
            }

        }


        SteamVR_Action_Boolean fireAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("fire");
        if (fireAction.stateDown || (useMouseForTesting && Input.GetMouseButtonDown(0)))
        {
            

           if(leftHand.GetDeviceIndex() == fireAction.trackedDeviceIndex)
            {
                selectedGun = leftHand.AttachedObjects[0].attachedObject;

            }else if(rightHand.GetDeviceIndex() == fireAction.trackedDeviceIndex)
            {
                selectedGun = rightHand.AttachedObjects[0].attachedObject;
            }

            Transform shellOrigin = selectedGun.GetComponent<PickUpWeapon>().shellOrigin;
            
            Rigidbody rb = Instantiate<Rigidbody>(shellPrefab.GetComponent<Rigidbody>(), shellOrigin.position, shellOrigin.rotation);

            // If this were attached to the gun itself, could use Vector3.right here instead, it'd be a helluvalot simpler
            rb.velocity =  Vector3.Normalize(-shellOrigin.forward); 

            if(!useMouseForTesting)
                SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic").Execute(0, vibrationDuration, vibrationFrequency, vibrationAmplitute, source);

            shotgunBlast.Play();
            
            
           

            RaycastHit hit = new RaycastHit();
            Transform rayOrigin = selectedGun.transform;

            bool is2dHit = useMouseForTesting && Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 300f, mask); // click to fire at the drones on desktop
            Ray r = new Ray(rayOrigin.position, rayOrigin.forward);
            if (is2dHit || Physics.Raycast(r, out hit, Mathf.Infinity, mask))
            {
                print("hit object name: " + hit.transform.name);
                TakeDamage td = hit.transform.GetComponent<TakeDamage>();

               

                /*ParticleSystem p = Instantiate<ParticleSystem>(sparkParticlePrefab.GetComponent<ParticleSystem>(), hit.point, Quaternion.LookRotation(hit.normal));

                ParticleSystem.MainModule main = p.main;
                ParticleSystem.Burst burst = new ParticleSystem.Burst(0, 10 * (100 - td.health));
                p.emission.SetBurst(0, burst);

                main.startSpeed = 160f * ((100 - td.health) / 25);
                main.startLifetime = .4f * (td.health / 100);*/
                

                td.takeDamage(hitDamage, hit, r, mask);
                hit.transform.GetComponent<AudioSource>().Play();

                /*print("textureCoord.x: " + hit.textureCoord.x);
                print("textureCoord.y: " + hit.textureCoord.y);
                */

            }
        
        }
    }

    public void OnObjectPickedUp()
    {
        
        print("OnObjectPickedup called");
        //selectedGun.transform.GetChild(0).position = 
        shootingActionSet.Activate();
        
    }

    public void OnObjectDropped()
    {
        print("OnObjectDropped called");
        
        defaultActionSet.Activate();
    }
}
