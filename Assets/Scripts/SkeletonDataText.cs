using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonDataText : MonoBehaviour
{
    public GameObject targetJoint;

    // Update is called once per frame
    void Update()
    {
        // Face the camera
        transform.forward = Camera.main.transform.forward;

        // Set position to above the target joint
        Vector3 targetJointPos = targetJoint.transform.position;
        targetJointPos.y += 0.25f;
        transform.position = targetJointPos;
    }
}
