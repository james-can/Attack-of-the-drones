using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class OverrideHandPose : MonoBehaviour
{
    public GameObject root;
    public SteamVR_Skeleton_Pose fistPose;
    void Update()
    {
        int i = 0;
        if (root != null)
        {
            Transform[] allChildren = root.gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                print("script is running");
                child.gameObject.transform.position = fistPose.rightHand.bonePositions[i];
                child.gameObject.transform.rotation = fistPose.rightHand.boneRotations[i];
                print(fistPose.rightHand.boneRotations[i]);
                i += 1;
            }
        }
    }
}
