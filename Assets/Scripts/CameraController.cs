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
        MoveAxis(Vector3.forward, KeyCode.W, KeyCode.S);
        MoveAxis(Vector3.right, KeyCode.D, KeyCode.A);
        MoveAxis(Vector3.up, KeyCode.Space, KeyCode.LeftShift);

        RotateAxis(Vector3.up, Space.World, KeyCode.RightArrow, KeyCode.LeftArrow);
        RotateAxis(Vector3.right, Space.Self, KeyCode.DownArrow, KeyCode.UpArrow);
    }

    void MoveAxis(Vector3 axis, KeyCode forwardKey, KeyCode backwardKey)
    {
        if (Input.GetKey(forwardKey))
        {
            transform.Translate(axis * moveSpeed);
        }
        if (Input.GetKey(backwardKey))
        {
            transform.Translate(axis * -moveSpeed);
        }
    }

    void RotateAxis(Vector3 axis, Space relativeTo, KeyCode clockwiseKey, KeyCode counterclockwiseKey)
    {
        if (Input.GetKey(clockwiseKey))
        {
            transform.Rotate(axis, rotateSpeed, relativeTo);
        }
        if (Input.GetKey(counterclockwiseKey))
        {
            transform.Rotate(axis, -rotateSpeed, relativeTo);
        }
    }
}
