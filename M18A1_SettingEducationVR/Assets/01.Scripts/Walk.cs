using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour
{
    public float walkSpeed = 3.0f;
    public float animSpeed = 1.4f;
    public float resetTime = 6.0f;
    private int count = 1;
    private Vector3 startPos;
    private Animation anim;

    void Start()
    {
        anim = GetComponent<Animation>();
        anim["Take 001"].speed = animSpeed;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
        if(Time.time > resetTime * count){
            transform.position = startPos;
            count++;
        }
    }


}
