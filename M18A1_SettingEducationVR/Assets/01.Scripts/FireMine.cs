using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMine : MonoBehaviour
{
    private ParticleSystem fireParticle;
    // Start is called before the first frame update
    void Start()
    {
        fireParticle = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstick)){
            fireParticle.Play();
        }
    }
}
