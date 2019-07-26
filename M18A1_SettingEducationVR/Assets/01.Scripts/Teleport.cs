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

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        playerTr = GameObject.Find("Player").transform;
        line = GetComponent<LineRenderer>();
        line.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!OVRInput.Get(OVRInput.NearTouch.SecondaryIndexTrigger)
            && !OVRInput.Get(OVRInput.NearTouch.SecondaryThumbButtons)
            && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            line.enabled = true;
        }
        // ray = new Ray(tr.position, tr.forward);
        // if (Physics.Raycast(ray, out hit, 10.0f))
        // {
        //     float dist = hit.distance;
        //     line.SetPosition(1, new Vector3(0, 0, dist));
        //     string hitTag = hit.collider.tag;
        //     if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        //     {
        //         switch (hitTag)
        //         {
        //             case "GROUND":
        //                 playerTr.position = hit.transform.position;
        //                 break;
        //             default:
        //                 break;
        //         }
        //     }
        // }
    }
}
