using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenMgr : MonoBehaviour
{
    private Transform myTr;

    void Start()
    {
        myTr = transform;
        
        iTween.MoveBy(gameObject, iTween.Hash("x", 5.3, "easeType", 
            iTween.EaseType.easeOutBack, "time", 2.0f, "delay", 1.0f));
    }

    public void PanelMove()
    {
        gameObject.transform.position = new Vector3(-110.2286f, 12.35f, 105.0753f);
        iTween.MoveBy(gameObject, iTween.Hash("x", 5.3, "easeType",
            iTween.EaseType.easeOutBack, "time", 2.0f, "delay", 1.0f));
    }
}
