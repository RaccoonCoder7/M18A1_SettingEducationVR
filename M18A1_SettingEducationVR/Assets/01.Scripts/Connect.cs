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
    public AudioClip separate;

    private void Start()
    {
        tr = GetComponent<Transform>();
        audio = GetComponent<AudioSource>();
    }

    private void LateUpdate()
    {
        if (!isConnected) return;
        float dist = Vector3.Distance(connectedObj.transform.position, tr.position);
        // if (dist < 1f)
        // {

        // }
        // else
        // {
        //     isConnected = false;
        // }
        Debug.Log(dist);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isConnected) return;
        if (other.tag == "IN")
        {
            OVRGrabbable grabbable = other.transform.parent.GetComponent<OVRGrabbable>();
            grabber.ForceRelease(grabbable);
            isConnected = true;
            connectedObj = other.gameObject;
            Transform anchor = tr.parent.Find("InAnchor");
            connectedObj.transform.parent.position = anchor.position;
            connectedObj.transform.parent.rotation = anchor.rotation;
            connectedObj.transform.parent.parent = anchor;
            audio.Play();
        }
        if (other.tag == "ROPE")
        {
            OVRGrabbable grabbable = other.gameObject.GetComponent<OVRGrabbable>();
            grabber.ForceRelease(grabbable);
            isConnected = true;
            connectedObj = other.gameObject;
            connectedObj.transform.position = tr.position;
            connectedObj.transform.rotation = tr.rotation;
            connectedObj.transform.parent = tr.parent;
            audio.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!isConnected) return;
        if (other.tag == "IN")
        {
            isConnected = false;
            connectedObj = null;
            other.transform.parent.parent = null;
            audio.PlayOneShot(separate);
        }
        if (other.tag == "ROPE")
        {
            isConnected = false;
            connectedObj = null;
            other.transform.parent = null;
            audio.PlayOneShot(separate);
        }
    }
}
