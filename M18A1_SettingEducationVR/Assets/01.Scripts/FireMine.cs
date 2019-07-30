using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMine : MonoBehaviour
{
    private ParticleSystem fireParticle;
    public DialogueMgr dialogueMgr;

    void Start()
    {
        fireParticle = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("DetonatorP/Back") && GameObject.Find("DetonatorP/Front") && !GameObject.Find("DetonatorP/InAnchor/ElectricTestP"))
        {
            Debug.Log("YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY   fire");
        }
        else
        {
            Debug.Log("NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN   fire");
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick)){
            fireParticle.Play();
            dialogueMgr.EndDrawing();
        }
    }
}
