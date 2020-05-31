using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RemoteConsole : MonoBehaviour
{
    public static RemoteConsole singleton;
    RemoteAdmin ra;

    public GameObject ConsoleRoot;
    public TextMeshProUGUI Content;

    public float Scale = 1;

    float yMin = -99840;
    float yMax = 99840;

    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (ConsoleRoot.activeSelf)
        {
            float y = -Input.mouseScrollDelta.y * Scale;
            if (y != 0)
            {
                Vector3 pos = Content.transform.localPosition;
                pos.y += y;

                if (pos.y < yMin)
                {
                    pos.y = yMin;
                }
                else if (pos.y > yMax)
                {
                    pos.y = yMax;
                }

                Content.transform.localPosition = pos;
            }
        }
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ConsoleRoot.SetActive(!ConsoleRoot.activeSelf);

            Cursor.lockState = ConsoleRoot.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = ConsoleRoot.activeSelf ? true : false;
        }
    }

    public void SendCommand(TMP_InputField tMP_InputField)
    {
        if(ra == null)
        {
            ra = FindObjectsOfType<RemoteAdmin>().ToList().Find(x => x.isLocalPlayer);
        }

        ra.CmdSendCmd(tMP_InputField.text);
    }

    /// <summary>
    /// Add console log with any color
    /// </summary>
    /// <param name="text"></param>
    /// <param name="color">Color of text</param>
    public void AddLogColor(string text, Color color)
    {
        Content.text += $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{DateTime()} {text}</color>" + Environment.NewLine;
    }

    /// <summary>
    /// Add console log with default color
    /// </summary>
    /// <param name="text"></param>
    public void AddLog(string text)
    {
        Content.text += $"{DateTime()} {text}" + Environment.NewLine;
    }

    public string DateTime()
    {
        return $"<b>{System.DateTime.Now.ToString("[HH:mm:ss]")}</b>";
    }
}