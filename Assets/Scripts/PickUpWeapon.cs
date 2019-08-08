using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class PickUpWeapon : Throwable
{
    public Hand rightHand;
    public Hand leftHand;
    public Transform shellOrigin;
    

    protected override void HandAttachedUpdate(Hand hand){
        // This is where an object would previously have been detached
        // Since this script is only attached to a pickable weapon,
        // Simply override, and omit the "code to detach"

        if (onHeldUpdate != null)
            onHeldUpdate.Invoke(hand);

    }
}
