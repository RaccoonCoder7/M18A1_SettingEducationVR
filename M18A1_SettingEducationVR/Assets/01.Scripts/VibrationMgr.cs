using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationMgr : MonoBehaviour
{
    public static VibrationMgr singleton;
    public AudioClip beep;

    void Start()
    {
        if (singleton && singleton != this)
        {
            Destroy(this);
        }
        else
        {
            singleton = this;
        }
    }

    public void TriggerVibration(OVRInput.Controller controller)
    {
        OVRHapticsClip ovrClip = new OVRHapticsClip(beep);

        if(controller == OVRInput.Controller.LTouch){
            OVRHaptics.LeftChannel.Preempt(ovrClip);
        }
        if(controller == OVRInput.Controller.RTouch){
            OVRHaptics.RightChannel.Preempt(ovrClip);
        }
    }
}
