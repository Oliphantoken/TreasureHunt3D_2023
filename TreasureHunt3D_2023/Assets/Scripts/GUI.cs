using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    public static GUI Instance;
    public Text messageText;
    public Text headerText;
    public Button OKButton;

    void Awake()
    {
        Instance = this;
        OKButton.enabled = false;
    }

    public void ShowHeader(string txt)
    {
        headerText.text = txt;
        
    }
    public void ShowMessage(string msg)
    {
        messageText.text = msg;
    }

    public void EnableInteraction(bool showing)
    {
        OKButton.enabled = showing;
    }

    public void ShowClue(int index, string message)
    {
        ShowHeader("Clue " + (index+1));
        ShowMessage(message);
    }


}
