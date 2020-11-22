using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using System.IO;

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

            // Create point body child
            Instantiate(pointBody, transform);
        }

        StartCoroutine("RenderSkeleton");
    }

    IEnumerator RenderSkeleton()
    {
        // Repeatedly render skeleton using joint position data in the input file
        while (curLine != null)
        {
            // Used to get time to wait before updating the point body
            float curTimestamp = float.Parse(rowArr[0]);
            float nextTimestamp = curTimestamp;
            
            // Iterate through current input file line
            // Joint positions start at column 7 and are in groups of 4
            for (int i = 0; i < (int)JointId.Count * 4; i += 4)
            {
                // Get a joint position from the line
                float curJointX = float.Parse(rowArr[i + 6].Remove(0, 2));
                float curJointY = -float.Parse(rowArr[i + 7]);
                float curJointZ = float.Parse(rowArr[i + 8].Remove(rowArr[8 + i].Length - 1, 1));
                Vector3 curJointPos = new Vector3(curJointX, curJointY, curJointZ);
            
                int jointNum = i / 4;
            
                // Set joint position to position in file
                transform.GetChild(0).GetChild(jointNum).transform.position = curJointPos;
            
                // Get bone corresponding to current joint
                Transform curBone = transform.GetChild(0).GetChild(jointNum).GetChild(0);
                if (parentJointMap[(JointId)jointNum] != JointId.Head && parentJointMap[(JointId)jointNum] != JointId.Count)
                {
                    // Set bone transform to correct position, rotation, and scale
                    Vector3 parentJointPos = transform.GetChild(0).GetChild((int)parentJointMap[(JointId)jointNum]).transform.position;
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
            
            // Get array of strings from next row of data
            curLine = sr.ReadLine();
            if (curLine != null)
            {
                rowArr = curLine.Split(',');

                // Get next row timestamp
                nextTimestamp = float.Parse(rowArr[0]);
            }
            
            // Wait for time between data collection from the file
            yield return new WaitForSeconds(nextTimestamp - curTimestamp);
        }

        // After the file has been read, destroy child point bodies
        foreach(Transform pointBody in transform)
        {
            Destroy(pointBody.gameObject);
        }
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
