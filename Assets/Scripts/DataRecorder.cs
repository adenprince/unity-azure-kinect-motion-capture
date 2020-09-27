using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Microsoft.Azure.Kinect.BodyTracking;

public class DataRecorder : MonoBehaviour
{
    string outputPath;

    // Start is called before the first frame update
    void Start()
    {
        outputPath = Application.dataPath + "/output.csv";
        File.WriteAllText(outputPath, "Time,ID,Left Elbow Angle,Right Elbow Angle,Left Knee Angle,Right Knee Angle\n");
    }

    public float getJointAngle(System.Numerics.Vector3[] jointPositions3D, int jointID)
    {
        System.Numerics.Vector3 v1 = jointPositions3D[jointID - 1] - jointPositions3D[jointID];
        System.Numerics.Vector3 v2 = jointPositions3D[jointID + 1] - jointPositions3D[jointID];

        float angleRad = Mathf.Acos(System.Numerics.Vector3.Dot(v1, v2) / (v1.Length() * v2.Length()));

        return angleRad * 180 / Mathf.PI;
    }

    public void collectData(Body skeleton)
    {
        float leftElbowAngle = getJointAngle(skeleton.JointPositions3D, (int)JointId.ElbowLeft);
        float rightElbowAngle = getJointAngle(skeleton.JointPositions3D, (int)JointId.ElbowRight);
        float leftKneeAngle = getJointAngle(skeleton.JointPositions3D, (int)JointId.KneeLeft);
        float rightKneeAngle = getJointAngle(skeleton.JointPositions3D, (int)JointId.KneeRight);

        File.AppendAllText(outputPath, Time.time + "," + skeleton.Id + "," + leftElbowAngle + "," +
                                       rightElbowAngle + "," + leftKneeAngle + "," + rightKneeAngle + "\n");
    }

}
