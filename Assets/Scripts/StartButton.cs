using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public Text errorText;
    public Text outputFileName;
    public GameObject[] disableOnClick;
    public GameObject[] enableOnClick;

    string errorMessages;

    public void onStartButtonClick()
    {
        if (!checkForErrors())
        {
            disableGameObjects();
            enableGameObjects();
        }
    }

    void addError(string error)
    {
        errorMessages += "Error: " + error + "\n";
    }

    bool checkForErrors()
    {
        errorMessages = "";

        if (outputFileName.text == "")
        {
            addError("Output file name cannot be empty.");
        }
        else if (System.IO.File.Exists(Application.dataPath + "/" + outputFileName.text))
        {
            addError("Output file \"" + outputFileName.text + "\" already exists.");
        }

        errorText.text = errorMessages;

        return errorMessages != "";
    }

    void disableGameObjects()
    {
        foreach (GameObject g in disableOnClick)
        {
            g.SetActive(false);
        }
    }

    void enableGameObjects()
    {
        foreach (GameObject g in enableOnClick)
        {
            g.SetActive(true);
        }
    }
}
