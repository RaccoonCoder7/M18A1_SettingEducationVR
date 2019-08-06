using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricTest : MonoBehaviour
{

    public Connect detonatorConn;
    public Connect electricTestConn;

    public OVRGrabber[] grabbers;
    private OVRGrabbable grabbedObject;
    public DialogueMgr dialogueMgr;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (detonatorConn.isConnected && electricTestConn.isConnected)
        {
            foreach (OVRGrabber grabber in grabbers)
            {
               if (grabber.grabbedObject != null)
               {
                   grabbedObject = grabber.grabbedObject;
               }
            }
            if (grabbedObject.name == "DetonatorP" && OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
            {
                dialogueMgr.CheckState();
                gameObject.GetComponent<MeshRenderer>().material.color = new Color(212, 114, 0);
                // VibrationMgr.singleton.TriggerVibration(OVRInput.Controller.RTouch);
                OVRInput.SetControllerVibration(0.4f, 0.4f, OVRInput.Controller.RTouch);
                StartCoroutine("ColorChange");
            }
            grabbedObject = null;
        }
    }

    IEnumerator ColorChange()
    {
        yield return new WaitForSeconds(2.0f);
        gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);
    }
}
