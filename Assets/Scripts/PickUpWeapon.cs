using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class PickUpWeapon : Throwable
{
    public Hand rightHand;
    public Hand leftHand;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("dropright").stateDown) 
        {
            print("drop (Grip button) action happend");
            rightHand.DetachObject(gameObject, false);
        }
        if (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("dropleft").stateDown)
        {
            print("drop (Grip button) action happend");
            leftHand.DetachObject(gameObject, false);
        }



    }

    protected override void HandAttachedUpdate(Hand hand){
        // This is where an object would previously have been detached
        // Since this script is only attached to a pickable weapon,
        // Simply override, and omit the "code to detach"

        if (onHeldUpdate != null)
            onHeldUpdate.Invoke(hand);

    }
}
