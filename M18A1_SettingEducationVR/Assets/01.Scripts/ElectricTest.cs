using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricTest : MonoBehaviour
{
    private OVRGrabbable grabbedObject;
    private OVRGrabber grabber;

    // Start is called before the first frame update
    void Start()
    {
        grabber = GameObject.Find("RightHandAnchor").GetComponent<OVRGrabber>();
        grabbedObject = grabber.grabbedObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Detonator/ElectricTest/Rope"))
        {
            Debug.Log("YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY   ElectricTest");
        }
        else
        {
            Debug.Log("NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN   ElectricTest");
        }
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
        {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color(212, 114, 0);
            StartCoroutine("ColorChange");
        }
    }

    IEnumerator ColorChange()
    {
        yield return new WaitForSeconds(2.0f);
        gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);
    }
}
