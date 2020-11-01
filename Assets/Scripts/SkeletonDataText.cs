using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonDataText : MonoBehaviour
{
    public Transform targetJoint;
    public Transform pelvis;
    public float xOffset;
    public float yOffset;

    // Update is called once per frame
    void Update()
    {
        // Face the camera
        transform.forward = Camera.main.transform.forward;

        // Set position to above the target joint and away from the pelvis
        Vector3 targetJointPos = targetJoint.position;
        targetJointPos.y += yOffset;
        targetJointPos.x += xOffset * Mathf.Sign(targetJoint.position.x - pelvis.position.x);
        transform.position = targetJointPos;
    }
}
