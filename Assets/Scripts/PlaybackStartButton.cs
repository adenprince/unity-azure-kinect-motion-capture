using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaybackStartButton : MonoBehaviour
{
    public Text errorText;
    public InputField inputFileName;
    public InputField sensorHeight;
    public Toggle emptyBackgroundToggle;
    public GameObject plane;
    public GameObject emptyBackground;
    public GameObject[] disableOnClick;
    public GameObject[] enableOnClick;

    string errorMessages;
    float sensorHeightVal;

    public void onStartButtonClick()
    {
        if (!checkForErrors())
        {
            if (emptyBackgroundToggle.isOn)
            {
                plane.SetActive(false);
                emptyBackground.SetActive(true);
            }

            // Move down plane and camera by sensor height
            plane.transform.Translate(sensorHeightVal * Vector3.down);
            Camera.main.transform.Translate(sensorHeightVal * Vector3.down);

            disableGameObjects();
            enableGameObjects();

            // Time starts when the start button is pressed
            Time.timeScale = 1.0f;
        }
    }

    void addError(string error)
    {
        errorMessages += "Error: " + error + "\n";
    }

    bool checkForErrors()
    {
        errorMessages = "";

        if (inputFileName.text == "")
        {
            addError("Input file name cannot be empty.");
        }
        else if (!System.IO.File.Exists(Application.dataPath + "/" + inputFileName.text))
        {
            addError("Input file \"" + inputFileName.text + "\" does not exist.");
        }

        // Try to convert sensor height input to float
        if (!float.TryParse(sensorHeight.text, out sensorHeightVal))
        {
            addError("Sensor height of " + sensorHeight.text + " is invalid.");
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
