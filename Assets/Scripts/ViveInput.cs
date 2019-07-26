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
            
            
            Rigidbody r = Instantiate<Rigidbody>(shellPrefab.GetComponent<Rigidbody>(), shellOrigin.position, shellOrigin.rotation);

            // If this were attached to the gun itself, could use Vector3.right here instead, it'd be a helluvalot simpler
            r.velocity =  Vector3.Normalize(shellDirectionVector.position - shellOrigin.position); 

            if(!useMouseForTesting)
                SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic").Execute(0, vibrationDuration, vibrationFrequency, vibrationAmplitute, source);

            shotgunBlast.Play();
            
            

            RaycastHit hit = new RaycastHit();
            
            bool is2dHit = useMouseForTesting && Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 300f, mask); // click to fire at the drones on desktop
            
            if (is2dHit || Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, 300f, mask))
            {
                Instantiate(sparkParticlePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                hit.transform.gameObject.GetComponents<AudioSource>()[0].Play();
                hit.transform.GetComponent<TakeDamage>().takeDamage(hitDamage, hit, rayOrigin.forward.normalized);

                print("textureCoord.x: " + hit.textureCoord.x);
                print("textureCoord.y: " + hit.textureCoord.y);
                print("hit object name: " + hit.transform.name);


                DamageLogicForShader[] _items = hit.transform.GetComponentsInChildren<DamageLogicForShader>(false);
                
                foreach(DamageLogicForShader _item in _items)
                {
                    _item.updateShaderDamage(new Vector4(hit.textureCoord.x, hit.textureCoord.y));
                }
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
