using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport2 : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private Transform tr;
    private Transform playerTr;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        playerTr = GameObject.Find("OVRPlayerController").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            // Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green);
            if (Physics.Raycast(ray, out hit, 30.0f))
            {
                string hitTag = hit.collider.tag;
                if (Input.GetMouseButtonUp(0))
                {
                    switch (hitTag)
                    {
                        case "GROUND":
                            playerTr.position = hit.transform.position;
                            playerTr.transform.position += playerTr.transform.up * 5f;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
