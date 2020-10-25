using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;

    // Update is called once per frame
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
            transform.Translate(axis * input * moveSpeed);
        }
    }

    void RotateAxis(Vector3 axis, Space relativeTo, float input)
    {
        if (input != 0.0f)
        {
            transform.Rotate(axis, input * rotateSpeed, relativeTo);
        }
    }
}
