using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    private LineRenderer line;
    private Ray ray;
    private RaycastHit hit;
    private Transform tr;
    private Transform playerTr;
    private GameObject pointerPrefab;
    private GameObject pointer;
    private float delayTime = 0.3f;
    private float prevTime;
    private OVRGrabber grabber;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        playerTr = GameObject.Find("Player").transform;
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        pointerPrefab = Resources.Load<GameObject>("Pointer");
        pointer = Instantiate<GameObject>(pointerPrefab);
        grabber = tr.parent.gameObject.GetComponent<OVRGrabber>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!OVRInput.Get(OVRInput.NearTouch.SecondaryIndexTrigger)
            && !OVRInput.Get(OVRInput.NearTouch.SecondaryThumbButtons)
            && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger)
            && grabber.m_grabbedObj == null)
        {
            line.enabled = true;
            ray = new Ray(tr.position, tr.forward);
            if (Physics.Raycast(ray, out hit, 30.0f))
            {
                if (!pointer.activeSelf)
                {
                    pointer.SetActive(true);
                }
                float dist = hit.distance;
                line.SetPosition(1, new Vector3(0, 0, dist));
                pointer.transform.position = tr.position + tr.forward * dist;
                pointer.transform.rotation = Quaternion.LookRotation(hit.normal);
                pointer.transform.position += pointer.transform.forward * 0.1f;
                string hitTag = hit.collider.tag;
                if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
                {
                    if (delayTime > Time.time - prevTime)
                    {
                        return;
                    }
                    switch (hitTag)
                    {
                        case "GROUND":
                            playerTr.position = pointer.transform.position;
                            break;
                        default:
                            break;
                    }
                    prevTime = Time.time;
                }
            }
            else
            {
                line.SetPosition(1, new Vector3(0, 0, 30.0f));
                pointer.SetActive(false);
            }
        }
        else
        {
            line.enabled = false;
            pointer.SetActive(false);
        }

    }
}
