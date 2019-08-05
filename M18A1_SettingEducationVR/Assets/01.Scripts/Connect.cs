using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect : MonoBehaviour
{
    private Transform tr;
    public GameObject connectedObj;
    public bool isConnected;
    private AudioSource audio;
    public OVRGrabber[] grabbers;
    public AudioClip separate;
    public DialogueMgr dialogueMgr;

    private void Start()
    {
        tr = GetComponent<Transform>();
        audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isConnected) return;
        if (other.tag == "IN")
        {
            OVRGrabbable grabbable = other.transform.parent.GetComponent<OVRGrabbable>();
            foreach (OVRGrabber grabber in grabbers)
            {
                grabber.ForceRelease(grabbable);
            }
            isConnected = true;
            connectedObj = other.gameObject;
            Transform anchor = tr.parent.Find("InAnchor");
            connectedObj.transform.parent.position = anchor.position;
            connectedObj.transform.parent.rotation = anchor.rotation;
            connectedObj.transform.parent.parent.parent = anchor;
            audio.Play();
            dialogueMgr.CheckState();
        }
        if (other.tag == "ROPE")
        {
            if (other.transform.parent == null
            || other.transform.parent.parent.name == "RopeTween"
            || other.transform.parent.name == "RightHandAnchor"
            || other.transform.parent.name == "LeftHandAnchor")
            {
                OVRGrabbable grabbable = other.gameObject.GetComponent<OVRGrabbable>();
                foreach (OVRGrabber grabber in grabbers)
                {
                    grabber.ForceRelease(grabbable);
                }
                isConnected = true;
                connectedObj = other.gameObject;
                connectedObj.transform.position = tr.position;
                connectedObj.transform.rotation = tr.rotation;
                connectedObj.transform.parent = tr.parent;
                audio.Play();
                dialogueMgr.CheckState();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isConnected) return;
        if (other.tag == "IN")
        {
            isConnected = false;
            connectedObj = null;
            other.transform.parent.parent.parent = null;
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
