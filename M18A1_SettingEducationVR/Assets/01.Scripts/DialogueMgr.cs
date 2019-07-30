using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

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
        ELineConnMine4,
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

        uiText = GameObject.Find("DialogueText").GetComponent<Text>();
        dialogObj = GameObject.Find("Dialog");

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
    }

    void CreateDialogueText(string dialogueText)
    {
        texts = dialogueText.Split('E');
    }

    void Update()
    {
        ray = new Ray(tr.position, tr.forward);
        if (Physics.Raycast(ray, out hit, 16.0f))
        {
            float dist = hit.distance;
            line.SetPosition(1, new Vector3(0, 0, dist));
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerBT))
            {
                StartCoroutine("Run");
            }
        }
    }

    void CheckState()
    {
        //if (GameObject.Find("DetonatorP/InAnchor/ElectricTestP") && mineState == MineState.Idle0)
        //{
        //    mineState = MineState.DetonConnETest1;
        //    EndDrawing();
        //}

        //if ((GameObject.Find("DetonatorP/InAnchor/ElectricTestP/Back")
        //    || GameObject.Find("DetonatorP/InAnchor/ElectricTestP/Front")) && mineState == MineState.DetonConnETest1)
        //{
        //    mineState = MineState.ETestConnELine2;
        //    EndDrawing();
        //}

        //if ((GameObject.Find("DetonatorP/InAnchor/ElectricTestP/Back")
        //    || GameObject.Find("DetonatorP/InAnchor/ElectricTestP/Front")) && OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick)
        //    && mineState == MineState.ETestConnELine2)
        //{
        //    mineState = MineState.ETestCheckLight3;
        //    EndDrawing();
        //}

        //if ((GameObject.Find("DetonatorP/InAnchor/ElectricTestP/Back")
        //    || GameObject.Find("DetonatorP/InAnchor/ElectricTestP/Front")) && OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick)
        //    && mineState == MineState.ETestConnELine2)
        //{
        //    mineState = MineState.ETestCheckLight3;
        //    EndDrawing();
        //}

        //if ((GameObject.Find("DetonatorP/Back") && GameObject.Find("ClaymoreMine/M18ClaymoreMine_Tornado_Studio/Front"))
        //    || (GameObject.Find("DetonatorP/Front") && GameObject.Find("ClaymoreMine/M18ClaymoreMine_Tornado_Studio/Back"))
        //    && !GameObject.Find("DetonatorP/InAnchor/ElectricTestP") && mineState == MineState.ReELineConnMine7)
        //{
        //    mineState = MineState.DetonConnELine8;
        //    EndDrawing();
        //}
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
            nextDialogue = 0;

            if (SkipNextCount == dialogueList.Count - 1)
            {
                SceneManager.LoadScene("StartScene");
            }
        }
    }

    IEnumerator PlayLine(string text)
    {
        nowState = State.Playing;
        for (int i = 0; i < text.Length + 1; i += 1)
        {
            yield return new WaitForSeconds(0.02f);
            uiText.text = text.Substring(0, i);
        }

        yield return new WaitForSeconds(0.5f);
        nowState = State.Next;
    }

    public void EndDrawing()
    {
        SkipNextCount++;
        CreateDialogueText(dialogueList[SkipNextCount]);
        dialogObj.SetActive(true);
        StartCoroutine("Run");
    }
}