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
    public CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        playerTr = GameObject.Find("Player").transform;
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        pointerPrefab = Resources.Load<GameObject>("Pointer");
        pointer = Instantiate<GameObject>(pointerPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (!OVRInput.Get(OVRInput.NearTouch.SecondaryIndexTrigger)
            && !OVRInput.Get(OVRInput.NearTouch.SecondaryThumbButtons)
            && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            line.enabled = true;
            pointer.SetActive(true);
            ray = new Ray(tr.position, tr.forward);
            if (Physics.Raycast(ray, out hit, 30.0f))
            {
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
                            Vector3 pos = pointer.transform.position;
                            // pos.y += 1.8f;
                            playerTr.position = pos;
                            break;
                        default:
                            break;
                    }
                    prevTime = Time.time;
                }
            }
        }
        else
        {
            line.enabled = false;
            pointer.SetActive(false);
        }

    }
}
