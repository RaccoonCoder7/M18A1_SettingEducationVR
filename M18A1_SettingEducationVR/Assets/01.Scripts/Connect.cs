using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect : MonoBehaviour
{
    private Transform tr;
    private GameObject connectedObj;
    private bool isConnected;
    private AudioSource audio;
    public OVRGrabber grabber;

    private void Start()
    {
        tr = GetComponent<Transform>();
        audio = GetComponent<AudioSource>();
    }

    private void LateUpdate()
    {
        if (!isConnected) return;
        float dist = Vector3.Distance(connectedObj.transform.position, tr.position);
        if (dist < 1f)
        {
            
        }
        else
        {
            isConnected = false;
        }
        Debug.Log(dist);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "IN")
        {
            OVRGrabbable grabbable = other.transform.parent.GetComponent<OVRGrabbable>();
            grabber.ForceRelease(grabbable);
            isConnected = true;
            connectedObj = other.gameObject;
            // connectedObj.transform.rotation = tr.transform.rotation;
            Transform anchor = tr.parent.Find("InAnchor");
            connectedObj.transform.parent.position = anchor.position;
            connectedObj.transform.parent.rotation = anchor.rotation;
            connectedObj.transform.parent.parent = anchor;
            
            audio.Play();
        }
    }
}
