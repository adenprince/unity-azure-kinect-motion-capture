using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserController : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;
    public GameObject cone; // Cone for toggling visibility
    public Text dataText;

    int screenshotIndex = 1;
    string screenshotPath;

    void Start()
    {
        screenshotPath = Application.dataPath + "/screenshot" + screenshotIndex + ".png";

        while (System.IO.File.Exists(screenshotPath))
        {
            ++screenshotIndex;
            screenshotPath = Application.dataPath + "/screenshot" + screenshotIndex + ".png";
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("ToggleCone"))
        {
            // Toggle whether the cone is active or not
            cone.SetActive(!cone.activeSelf);
        }

        if (Input.GetButtonDown("ResetCamera"))
        {
            // Set camera position to origin and reset rotation
            Camera.main.transform.position = Vector3.zero;
            Camera.main.transform.rotation = Quaternion.identity;
        }

        if (Input.GetButtonDown("Screenshot"))
        {
            // Take screenshot
            ScreenCapture.CaptureScreenshot(screenshotPath);
            Debug.Log("Screenshot saved in \"" + screenshotPath + "\"");

            // Increment screenshot name index
            ++screenshotIndex;
            screenshotPath = Application.dataPath + "/screenshot" + screenshotIndex + ".png";
        }

        dataText.text = "Time: " + (int)Time.time;
    }

    void FixedUpdate()
    {
        MoveAxis(Vector3.forward, Input.GetAxis("CamForward"));
        MoveAxis(Vector3.right, Input.GetAxis("CamHorizontal"));
        MoveAxis(Vector3.up, Input.GetAxis("CamVertical"));

        RotateAxis(Vector3.up, Space.World, Input.GetAxis("CamRotY"));
        RotateAxis(Vector3.right, Space.Self, Input.GetAxis("CamRotX"));
    }

    void MoveAxis(Vector3 axis, float input)
    {
        if (input != 0.0f)
        {
            Camera.main.transform.Translate(axis * input * moveSpeed);
        }
    }

    void RotateAxis(Vector3 axis, Space relativeTo, float input)
    {
        if (input != 0.0f)
        {
            Camera.main.transform.Rotate(axis, input * rotateSpeed, relativeTo);
        }
    }
}
