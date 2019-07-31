using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claymore : MonoBehaviour
{
    private int leaf;
    private bool isConnected;
    private bool isSet;
    private GameObject connectedObj;
    private AudioSource audio;
    private Transform anchor;
    private Ray ray;
    private RaycastHit hit;
    private GameObject setPointObj;
    private List<Transform> rayPoints = new List<Transform>();
    private List<MeshRenderer> cylinders = new List<MeshRenderer>();
    public OVRGrabber[] grabbers;
    public ParticleSystem fireParticle;
    public AudioClip separate;
    public AudioClip fire;
    public Connect detonatorConn;
    public Connect electricTestConn;

    // Start is called before the first frame update
    void Start()
    {
        leaf = 1 << LayerMask.NameToLayer("LEAF");
        audio = GetComponent<AudioSource>();
        anchor = transform.Find("Anchor");
        Transform rays = transform.Find("Rays");
        foreach (Transform child in rays)
        {
            MeshRenderer rend = child.Find("Cylinder").GetComponent<MeshRenderer>();
            rend.enabled = false;
            cylinders.Add(rend);
            rayPoints.Add(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isSet)
        {
            if (!CheckClaymoreHidden())
            {
                return;
            }

            if (detonatorConn.isConnected && this.isConnected
                && !electricTestConn.isConnected && setPointObj != null)
            {
                if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
                {
                    fireParticle.Play();
                    audio.PlayOneShot(fire);
                    Destroy(setPointObj);
                    setPointObj = null;
                }
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ROPE")
        {
            if (other.transform.parent == null
            || other.transform.parent.name == "RopeTween"
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
                connectedObj.GetComponent<BoxCollider>().enabled = false;
                connectedObj.transform.position = anchor.position;
                connectedObj.transform.parent = anchor.parent;
                audio.Play();
            }
        }
        if (other.tag == "SETPOINT")
        {
            setPointObj = other.gameObject;
            setPointObj.GetComponent<MeshRenderer>().enabled = false;
            setPointObj.GetComponent<SphereCollider>().enabled = false;
            OVRGrabbable grabbable = transform.parent.GetComponent<OVRGrabbable>();
            foreach (OVRGrabber grabber in grabbers)
            {
                grabber.ForceRelease(grabbable);
            }
            isSet = true;
            transform.parent.position = other.transform.position;
            transform.parent.rotation = other.transform.rotation;
            transform.parent.GetComponent<OVRGrabbable>().enabled = false;
            transform.parent.GetComponent<BoxCollider>().enabled = false;
            transform.parent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
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

    private bool CheckClaymoreHidden()
    {
        for (int i = 0; i < rayPoints.Count; i++)
        {
            ray = new Ray(rayPoints[i].position, rayPoints[i].up);
            Debug.DrawRay(rayPoints[i].position, rayPoints[i].up, Color.green, 0.4f);
            if (Physics.Raycast(ray, out hit, 0.4f, leaf))
            {
                cylinders[i].enabled = false;
            }
            else
            {
                cylinders[i].enabled = true;
                return false;
            }
        }
        return true;
    }
}
