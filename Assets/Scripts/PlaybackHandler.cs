using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using System.IO;
using System;
using TMPro;

public class PlaybackHandler : MonoBehaviour
{
    public GameObject pointBody;

    string inputFileName;
    string curLine;
    string[] rowArr;
    StreamReader sr;
    Dictionary<JointId, JointId> parentJointMap;

    // Start is called before the first frame update
    void Start()
    {
        initParentJointMap();

        inputFileName = Application.dataPath + "/" + inputFileName;
        sr = new StreamReader(inputFileName);

        // Skip first line of file
        sr.ReadLine();

        // Get array of strings from first row of data
        curLine = sr.ReadLine();
        if (curLine != null)
        {
            rowArr = curLine.Split(',');
        }

        StartCoroutine("RenderAllSkeletons");
    }

    IEnumerator RenderAllSkeletons()
    {
        int numChildren = 0;

        // Skip beginning lines without body data
        while (curLine != null && rowArr[2] == "") {
            // Get array of strings from next row of data
            curLine = sr.ReadLine();
            if (curLine != null)
            {
                rowArr = curLine.Split(',');
            }
        }

        // Repeatedly render skeleton using joint position data in the input file
        while (curLine != null)
        {
            float startTime = Time.realtimeSinceStartup;

            // Used to get time to wait before updating the point body
            float curTimestamp = float.Parse(rowArr[1]);
            float nextTimestamp = curTimestamp;

            int curFrame = int.Parse(rowArr[0]);
            int curChild = 0;

            // Render each skeleton on the current frame
            while (curFrame == int.Parse(rowArr[0]) && curLine != null)
            {
                // Check if current line has body data
                if (rowArr[2] != "")
                {
                    // Create new point body child if needed
                    if (curChild + 1 > numChildren)
                    {
                        Instantiate(pointBody, transform);
                        ++numChildren;
                    }

                    RenderSkeleton(rowArr, curChild);

                    ++curChild;
                }

                // Get array of strings from next row of data
                curLine = sr.ReadLine();
                if (curLine != null)
                {
                    rowArr = curLine.Split(',');
                }
            }

            // Destroy unused point body children
            while (numChildren > curChild)
            {
                Destroy(transform.GetChild(numChildren - 1).gameObject);
                --numChildren;
            }

            if (curLine != null)
            {
                // Get next frame timestamp
                nextTimestamp = float.Parse(rowArr[1]);
            }
            
            // Wait for remaining time between data collection from the file
            yield return new WaitForSeconds(nextTimestamp - curTimestamp - (Time.realtimeSinceStartup - startTime));
        }

        // After the file has been read, destroy child point bodies
        foreach(Transform pointBody in transform)
        {
            Destroy(pointBody.gameObject);
        }
    }

    void RenderSkeleton(string[] rowArr, int childNumber)
    {
        // Iterate through current input file line
        // Joint positions start at column 8 and are in groups of 4
        for(int i = 0; i < (int)JointId.Count * 4; i += 4)
        {
            // Get a joint position from the line
            float curJointX = float.Parse(rowArr[i + 7].Remove(0, 2));
            float curJointY = -float.Parse(rowArr[i + 8]);
            float curJointZ = float.Parse(rowArr[i + 9].Remove(rowArr[9 + i].Length - 1, 1));
            Vector3 curJointPos = new Vector3(curJointX, curJointY, curJointZ);

            int jointNum = i / 4;

            // Set joint position to position in file
            transform.GetChild(childNumber).GetChild(jointNum).transform.position = curJointPos;

            // Get bone corresponding to current joint
            Transform curBone = transform.GetChild(childNumber).GetChild(jointNum).GetChild(0);
            if(parentJointMap[(JointId)jointNum] != JointId.Head && parentJointMap[(JointId)jointNum] != JointId.Count)
            {
                // Set bone transform to correct position, rotation, and scale
                Vector3 parentJointPos = transform.GetChild(childNumber).GetChild((int)parentJointMap[(JointId)jointNum]).transform.position;
                curBone.position = (curJointPos + parentJointPos) / 2;
                curBone.transform.up = curJointPos - parentJointPos;
                Vector3 boneScale = new Vector3(1.0f, Vector3.Distance(curJointPos, parentJointPos) * 10.0f, 1.0f);
                curBone.localScale = boneScale;
            }
            else
            {
                // Bones with the parent head or count should not be rendered
                curBone.gameObject.SetActive(false);
            }
        }

        Action<int, float> setAngleText = delegate (int Id, float angle)
        {
            transform.GetChild(childNumber).GetChild(Id).GetChild(1)
                     .GetComponent<TextMeshPro>().text = angle.ToString("F0") + "°";
        };

        // Update rendered text with angle data
        setAngleText((int)JointId.ElbowLeft, float.Parse(rowArr[3]));
        setAngleText((int)JointId.ElbowRight, float.Parse(rowArr[4]));
        setAngleText((int)JointId.KneeLeft, float.Parse(rowArr[5]));
        setAngleText((int)JointId.KneeRight, float.Parse(rowArr[6]));

        // Update skeleton ID text
        transform.GetChild(childNumber).GetChild((int)JointId.Count).GetComponent<TextMeshPro>().text = rowArr[2];
    }

    public void setInputFileName(string inputFileName)
    {
        this.inputFileName = inputFileName;
    }

    // Fill parent joint map (from TrackerHandler.cs)
    void initParentJointMap()
    {
        parentJointMap = new Dictionary<JointId, JointId>();

        // pelvis has no parent so set to count
        parentJointMap[JointId.Pelvis] = JointId.Count;
        parentJointMap[JointId.SpineNavel] = JointId.Pelvis;
        parentJointMap[JointId.SpineChest] = JointId.SpineNavel;
        parentJointMap[JointId.Neck] = JointId.SpineChest;
        parentJointMap[JointId.ClavicleLeft] = JointId.SpineChest;
        parentJointMap[JointId.ShoulderLeft] = JointId.ClavicleLeft;
        parentJointMap[JointId.ElbowLeft] = JointId.ShoulderLeft;
        parentJointMap[JointId.WristLeft] = JointId.ElbowLeft;
        parentJointMap[JointId.HandLeft] = JointId.WristLeft;
        parentJointMap[JointId.HandTipLeft] = JointId.HandLeft;
        parentJointMap[JointId.ThumbLeft] = JointId.HandLeft;
        parentJointMap[JointId.ClavicleRight] = JointId.SpineChest;
        parentJointMap[JointId.ShoulderRight] = JointId.ClavicleRight;
        parentJointMap[JointId.ElbowRight] = JointId.ShoulderRight;
        parentJointMap[JointId.WristRight] = JointId.ElbowRight;
        parentJointMap[JointId.HandRight] = JointId.WristRight;
        parentJointMap[JointId.HandTipRight] = JointId.HandRight;
        parentJointMap[JointId.ThumbRight] = JointId.HandRight;
        parentJointMap[JointId.HipLeft] = JointId.SpineNavel;
        parentJointMap[JointId.KneeLeft] = JointId.HipLeft;
        parentJointMap[JointId.AnkleLeft] = JointId.KneeLeft;
        parentJointMap[JointId.FootLeft] = JointId.AnkleLeft;
        parentJointMap[JointId.HipRight] = JointId.SpineNavel;
        parentJointMap[JointId.KneeRight] = JointId.HipRight;
        parentJointMap[JointId.AnkleRight] = JointId.KneeRight;
        parentJointMap[JointId.FootRight] = JointId.AnkleRight;
        parentJointMap[JointId.Head] = JointId.Pelvis;
        parentJointMap[JointId.Nose] = JointId.Head;
        parentJointMap[JointId.EyeLeft] = JointId.Head;
        parentJointMap[JointId.EarLeft] = JointId.Head;
        parentJointMap[JointId.EyeRight] = JointId.Head;
        parentJointMap[JointId.EarRight] = JointId.Head;
    }
}
