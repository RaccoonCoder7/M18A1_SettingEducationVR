using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class DialogueMgr : MonoBehaviour
{
    private Transform tr;
    private LineRenderer line;

    private Ray ray;
    private RaycastHit hit;
    private Camera cam;
    private int layerBT;

    private string[] texts;
    private List<string> dialogueList;
    private Text uiText;
    private State nowState;
    private MineState mineState;
    private int nextDialogue = 0;
    private int SkipNextCount = 0;

    private GameObject dialogObj;
    private VideoPlayer videoPlayer;
    public VideoClip[] videoClips;

    public Connect detonatorConn;
    public Connect electricTestConn;
    public Claymore claymoreConn;
    private ArrowRenderer arrowRenderer;
    private Vector3 claymoreSetPoint;
    private bool showArrow;

    public OVRGrabber[] grabbers;
    private OVRGrabbable grabbedObject;

    private GameObject okCanvas;
    private GameObject ground;

    private Vector3 originPos;
    private Quaternion originRot;

    private AudioSource audio;

    public bool isSet = false;
    public bool isHidden = false;

    enum State
    {
        Idle,
        Playing,
        Next,
    }

    enum MineState
    {
        Idle0,
        DetonConnETest1,
        ETestConnELine2,
        ETestCheckLight3,
        //ELineConnMine4, 3번과 합쳐짐
        MineSet5,
        MineHide6,
        ReELineConnMine7,
        DetonConnELine8,
        Fire9,
    }

    void Start()
    {
        tr = GetComponent<Transform>();
        line = GetComponent<LineRenderer>();
        layerBT = 1 << LayerMask.NameToLayer("DIALOGUE");
        cam = Camera.main;

        dialogueList = new List<string>();

        arrowRenderer = GameObject.Find("ArrowRenderer").GetComponent<ArrowRenderer>();
        claymoreSetPoint = GameObject.Find("ClaymoreSetPoint").transform.position;

        uiText = GameObject.Find("DialogueText").GetComponent<Text>();
        dialogObj = GameObject.Find("DialogCanvas");
        videoPlayer = dialogObj.transform.parent.Find("VideoPlayer").GetComponent<VideoPlayer>();

        audio = GetComponent<AudioSource>();

        TextAsset data = Resources.Load("DialogueText", typeof(TextAsset)) as TextAsset;
        StringReader sr = new StringReader(data.text);

        string dialogueLine;
        dialogueLine = sr.ReadLine();
        while (dialogueLine != null)
        {
            dialogueList.Add(dialogueLine);
            dialogueLine = sr.ReadLine();
        }

        CreateDialogueText(dialogueList[SkipNextCount]);

        nowState = State.Next;
        mineState = MineState.Idle0;

        ground = GameObject.Find("LakeTerrain");
        StartCoroutine("GroundTag", "Untagged");

        okCanvas = GameObject.Find("OkCanvas");
        okCanvas.SetActive(false);
        GrabberChange(false);
    }

    void CreateDialogueText(string dialogueText)
    {
        texts = dialogueText.Split('E');
    }

    void Update()
    {
        ray = new Ray(tr.position, tr.forward);
        if (showArrow)
        {
            if (Vector3.Distance(claymoreConn.gameObject.transform.position, claymoreSetPoint) > 3f)
            {
                arrowRenderer.SetPositions(claymoreConn.gameObject.transform.position, claymoreSetPoint);
            }
            else
            {
                arrowRenderer.SetPositions(Vector3.zero, Vector3.zero);
                showArrow = false;
            }
        }
        if (Physics.Raycast(ray, out hit, 16.0f))
        {
            float dist = hit.distance;
            line.SetPosition(1, new Vector3(0, 0, dist));
        }
        if (!OVRInput.Get(OVRInput.NearTouch.SecondaryIndexTrigger)
            && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            if (OVRInput.GetUp(OVRInput.Button.One))
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerBT))
                {
                    if (mineState == MineState.Idle0)
                    {
                        videoPlayer.gameObject.SetActive(false);
                    }
                    StartCoroutine("Run");
                }
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("NEXT")))
                {
                    switch (mineState)
                    {
                        case MineState.Idle0:
                            OutlineOnOff("DetonatorP", true);
                            OutlineOnOff("ElectricTestP", true);
                            break;
                        case MineState.DetonConnETest1:
                            OutlineOnOff("RopeTween", true);
                            OutlineOnOff("M18ClaymoreMine", true);
                            OutlineOnOff("ElectricTestP", true);
                            break;
                        case MineState.ETestConnELine2:
                            OutlineOnOff("DetonatorP", true);
                            OutlineOnOff("ElectricTestLight", true);
                            videoPlayer.gameObject.SetActive(true);
                            videoPlayer.clip = videoClips[1];
                            break;
                        case MineState.ETestCheckLight3:
                            StartCoroutine("GroundTag", "GROUND");
                            showArrow = true;
                            break;
                        case MineState.MineSet5:
                            StartCoroutine("GroundTag", "GROUND");
                            break;
                        case MineState.MineHide6:
                            StartCoroutine("GroundTag", "GROUND");
                            OutlineOnOff("DetonatorP", true);
                            OutlineOnOff("ElectricTestLight", true);
                            break;
                        case MineState.ReELineConnMine7:
                            OutlineOnOff("RopeTween", true);
                            OutlineOnOff("ElectricTestP", true);
                            OutlineOnOff("DetonatorP", true);
                            GameObject.Find("ElectricTestP").GetComponent<Outline>().OutlineColor = new Color(255, 0, 0);
                            break;
                        case MineState.DetonConnELine8:
                            OutlineOnOff("DetonatorP", true);
                            break;
                    }
                    GrabberChange(true);
                    okCanvas.SetActive(false);
                }
            }
        }
    }

    IEnumerator GroundTag(string tagName)
    {
        yield return new WaitForSeconds(0.5f);
        ground.tag = tagName;
    }

    private void OutlineOnOff(string objectName, bool onOff)
    {
        GameObject.Find(objectName).GetComponent<Outline>().enabled = onOff;
    }

    private void GrabberChange(bool state)
    {
        foreach (OVRGrabber grabber in grabbers)
        {
            if (state)
            {
                grabber.grabBegin = 0.55f;
                grabber.grabEnd = 0.35f;
            }
            else
            {
                grabber.grabBegin = 0.0f;
                grabber.grabEnd = 0.0f;
            }
        }
    }

    private bool CheckGrapDetonator()
    {
        foreach (OVRGrabber grabber in grabbers)
        {
            if (grabber.grabbedObject != null)
            {
                grabbedObject = grabber.grabbedObject;
            }
        }

        return grabbedObject.name == "DetonatorP";
    }

    public void CheckState()
    {
        if (detonatorConn.isConnected && detonatorConn.connectedObj.tag == "IN" && mineState == MineState.Idle0)
        {
            mineState = MineState.DetonConnETest1;
            OutlineOnOff("DetonatorP", false);
            OutlineOnOff("ElectricTestP", false);
            EndDrawing();
        }

        if (detonatorConn.isConnected && detonatorConn.connectedObj.tag == "IN" && electricTestConn.isConnected
            && claymoreConn.isConnected && mineState == MineState.DetonConnETest1)
        {
            mineState = MineState.ETestConnELine2;
            OutlineOnOff("M18ClaymoreMine", false);
            OutlineOnOff("RopeTween", false);
            OutlineOnOff("ElectricTestP", false);
            videoPlayer.gameObject.SetActive(false);
            EndDrawing();
        }

        if (detonatorConn.isConnected && electricTestConn.isConnected && claymoreConn.isConnected
            && detonatorConn.connectedObj.tag == "IN" && OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick)
            && mineState == MineState.ETestConnELine2)
        {
            if (CheckGrapDetonator())
            {
                mineState = MineState.ETestCheckLight3;
                OutlineOnOff("DetonatorP", false);
                OutlineOnOff("ElectricTestLight", false);
                videoPlayer.gameObject.SetActive(true);
                videoPlayer.clip = videoClips[0];
                EndDrawing();
            }
            grabbedObject = null;
        }

        if (this.isSet && mineState == MineState.ETestCheckLight3)
        {
            mineState = MineState.MineSet5;
            videoPlayer.gameObject.SetActive(false);
            ground.tag = "Untagged";
            originPos = dialogObj.transform.parent.position;
            originRot = dialogObj.transform.parent.rotation;
            Transform anchorTr = GameObject.Find("CanvasAnchor").transform;
            dialogObj.transform.parent.position = anchorTr.position;
            dialogObj.transform.parent.rotation = anchorTr.rotation;
            EndDrawing();
        }

        if (this.isHidden && mineState == MineState.MineSet5)
        {
            mineState = MineState.MineHide6;
            ground.tag = "Untagged";
            EndDrawing();
        }

        if (detonatorConn.isConnected && electricTestConn.isConnected && claymoreConn.isConnected
            && detonatorConn.connectedObj.tag == "IN" && OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick)
            && mineState == MineState.MineHide6)
        {
            if (CheckGrapDetonator())
            {
                mineState = MineState.ReELineConnMine7;
                OutlineOnOff("DetonatorP", false);
                OutlineOnOff("ElectricTestLight", false);
                ground.tag = "Untagged";
                dialogObj.transform.parent.position = originPos;
                dialogObj.transform.parent.rotation = originRot;
                EndDrawing();
            }
            grabbedObject = null;
        }

        if (detonatorConn.isConnected && !electricTestConn.isConnected && claymoreConn.isConnected
            && detonatorConn.connectedObj.tag == "ROPE" && mineState == MineState.ReELineConnMine7)
        {
            GameObject enemies = Resources.Load("Enemies") as GameObject;
            Instantiate(enemies);
            mineState = MineState.DetonConnELine8;
            OutlineOnOff("RopeTween", false);
            OutlineOnOff("ElectricTestP", false);
            OutlineOnOff("DetonatorP", false);
            EndDrawing();
        }

        if (detonatorConn.isConnected && !electricTestConn.isConnected && claymoreConn.isConnected
            && detonatorConn.connectedObj.tag == "ROPE"
            && OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick) && mineState == MineState.DetonConnELine8)
        {
            if (CheckGrapDetonator())
            {
                mineState = MineState.Fire9;
                OutlineOnOff("DetonatorP", false);
                EndDrawing();
            }
            grabbedObject = null;
        }
    }

    IEnumerator Run()
    {
        if (nextDialogue < texts.Length && nowState == State.Next)
        {
            yield return PlayLine(texts[nextDialogue]);
            nextDialogue++;
        }
        else if (nextDialogue == texts.Length)
        {
            dialogObj.SetActive(false);
            okCanvas.SetActive(true);
            nextDialogue = 0;

            if (SkipNextCount == dialogueList.Count - 1)
            {
                SceneManager.LoadScene("race_track_lake");
                okCanvas.SetActive(false);
            }
        }
    }

    IEnumerator PlayLine(string text)
    {
        nowState = State.Playing;
        for (int i = 0; i < text.Length + 1; i++)
        {
            yield return new WaitForSeconds(0.02f);
            Debug.LogError("00: " + i);
            if (i != 0 && text.Substring(i - 1, 1) == "<")
            {
                i = getEndOfTag(text, i);
                if (i == 0) Debug.LogError("getEndOfTag Error");
            }
            uiText.text = text.Substring(0, i);
        }

        yield return new WaitForSeconds(0.5f);
        nowState = State.Next;
    }

    private int getEndOfTag(string text, int i)
    {
        int count = 0;
        for (int j = i; j < text.Length + 1; j++)
        {
            if (text.Substring(j, 1) == ">")
            {
                count++;
                if (count == 2)
                {
                    return j + 1;
                }
            }
        }
        return 0;
    }

    public void EndDrawing()
    {
        audio.Play();
        SkipNextCount++;
        CreateDialogueText(dialogueList[SkipNextCount]);
        dialogObj.SetActive(true);
        GrabberChange(false);
        StartCoroutine("Run");
    }
}