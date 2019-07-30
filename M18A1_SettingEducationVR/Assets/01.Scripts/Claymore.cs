using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claymore : MonoBehaviour
{
    private bool isConnected;
    private GameObject connectedObj;
    private AudioSource audio;
    private Transform anchor;
    public OVRGrabber[] grabbers;
    public ParticleSystem fireParticle;
    public AudioClip separate;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        anchor = transform.Find("Anchor");
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick) && isConnected)
        {
            fireParticle.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isConnected) return;
        if (other.tag == "ROPE")
        {
            if (other.transform.parent == null
            || other.transform.parent.name == "RopeTween"
            || other.transform.parent.name == "RightHandAnchor")
            {
                OVRGrabbable grabbable = other.gameObject.GetComponent<OVRGrabbable>();
                foreach (OVRGrabber grabber in grabbers)
                {
                    grabber.ForceRelease(grabbable);
                }
                isConnected = true;
                connectedObj = other.gameObject;
                connectedObj.transform.position = anchor.position;
                connectedObj.transform.parent = anchor.parent;
                audio.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isConnected) return;
        if (other.tag == "ROPE")
        {
            isConnected = false;
            connectedObj = null;
            other.transform.parent = null;
            audio.PlayOneShot(separate);
        }
    }
}
