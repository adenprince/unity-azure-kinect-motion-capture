using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Microsoft.Azure.Kinect.BodyTracking;
using System;
using TMPro;

public class DataRecorder : MonoBehaviour
{
    public Text dataText;

    bool writeData = true;
    string outputPath;
    string fileName;
    string displayedData;

    // Start is called before the first frame update
    void Start()
    {
        outputPath = Application.dataPath + "/" + fileName;

        if (writeData)
        {
            File.WriteAllText(outputPath, "Frame,Time,ID,Left Elbow Angle,Right Elbow Angle,Left Knee Angle,Right Knee Angle,");

            // Write column names for joint positions
            for(int i = 0; i < (int)JointId.Count - 1; ++i)
            {
                File.AppendAllText(outputPath, ((JointId)i).ToString() + " Pos,");
            }
            File.AppendAllText(outputPath, JointId.EarRight.ToString() + " Pos\n");
        }
    }

    public float getJointAngle(System.Numerics.Vector3[] jointPositions3D, int jointID)
    {
        System.Numerics.Vector3 v1 = jointPositions3D[jointID - 1] - jointPositions3D[jointID];
        System.Numerics.Vector3 v2 = jointPositions3D[jointID + 1] - jointPositions3D[jointID];

        float angleRad = Mathf.Acos(System.Numerics.Vector3.Dot(v1, v2) / (v1.Length() * v2.Length()));

        return angleRad * 180 / Mathf.PI;
    }

    public void collectData(Body skeleton, int skeletonNumber, int frameNumber)
    {
        // Get joint angle data
        float leftElbowAngle = getJointAngle(skeleton.JointPositions3D, (int)JointId.ElbowLeft);
        float rightElbowAngle = getJointAngle(skeleton.JointPositions3D, (int)JointId.ElbowRight);
        float leftKneeAngle = getJointAngle(skeleton.JointPositions3D, (int)JointId.KneeLeft);
        float rightKneeAngle = getJointAngle(skeleton.JointPositions3D, (int)JointId.KneeRight);

        if (writeData)
        {
            // Write joint angle data to file
            File.AppendAllText(outputPath, frameNumber + "," + Time.time + "," + skeleton.Id + "," +
                                           leftElbowAngle + "," + rightElbowAngle + "," +
                                           leftKneeAngle + "," + rightKneeAngle + ",");

            // Write position and distance data for each joint to the output file
            System.Numerics.Vector3 curJoint;
            for (int i = 0; i < (int)JointId.Count - 1; ++i)
            {
                curJoint = skeleton.JointPositions3D[i];
                File.AppendAllText(outputPath, "\"" + curJoint.ToString() + ", " + curJoint.Length() + "\",");
            }
            // Write last joint position and distance data
            curJoint = skeleton.JointPositions3D[(int)JointId.EarRight];
            File.AppendAllText(outputPath, "\"" + curJoint.ToString() + ", " + curJoint.Length() + "\"\n");
        }

        Action<int, float> setAngleText = delegate(int Id, float angle)
        {
            transform.GetChild(skeletonNumber).GetChild(Id).GetChild(1)
                     .GetComponent<TextMeshPro>().text = angle.ToString("F0") + "°";
        };

        // Update rendered text with angle data
        setAngleText((int)JointId.ElbowLeft, leftElbowAngle);
        setAngleText((int)JointId.ElbowRight, rightElbowAngle);
        setAngleText((int)JointId.KneeLeft, leftKneeAngle);
        setAngleText((int)JointId.KneeRight, rightKneeAngle);
    }

    public void resetDisplayedData()
    {
        this.displayedData = "Time: " + (int)Time.time;

        dataText.text = displayedData;
    }

    public string getFileName()
    {
        return fileName;
    }

    public void setDefaultFileName()
    {
        int fileNameIndex = 1;

        fileName = "output" + fileNameIndex + ".csv";

        while (System.IO.File.Exists(Application.dataPath + "/" + fileName))
        {
            ++fileNameIndex;
            fileName = "output" + fileNameIndex + ".csv";
        }
    }

    public void setFileName(string fileName)
    {
        this.fileName = fileName;
    }

    public void setWriteData(bool writeData)
    {
        this.writeData = writeData;
    }
}
