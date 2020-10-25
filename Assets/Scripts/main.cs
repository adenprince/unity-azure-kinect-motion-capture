using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;

public class main : MonoBehaviour
{
    // Handler for SkeletalTracking thread.
    public GameObject m_tracker;
    private BackgroundDataProvider m_backgroundDataProvider;
    public BackgroundData m_lastFrameData = new BackgroundData();

    public FPS frameRate = FPS.FPS30;
    public DepthMode depthMode = DepthMode.NFOV_Unbinned;
    public WiredSyncMode wiredSyncMode = WiredSyncMode.Standalone;

    int screenshotIndex = 1;
    string screenshotPath;
    
    void Start()
    {
        SkeletalTrackingProvider m_skeletalTrackingProvider = new SkeletalTrackingProvider(frameRate, depthMode, wiredSyncMode);

        //tracker ids needed for when there are two trackers
        const int TRACKER_ID = 0;
        m_skeletalTrackingProvider.StartClientThread(TRACKER_ID);
        m_backgroundDataProvider = m_skeletalTrackingProvider;

        screenshotPath = Application.dataPath + "/screenshot" + screenshotIndex + ".png";

        while (System.IO.File.Exists(screenshotPath))
        {
            ++screenshotIndex;
            screenshotPath = Application.dataPath + "/screenshot" + screenshotIndex + ".png";
        }
    }

    void Update()
    {
        if (m_backgroundDataProvider.IsRunning)
        {
            if (m_backgroundDataProvider.GetCurrentFrameData(ref m_lastFrameData))
            {
                m_tracker.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData);
            }
        }

        if (Input.GetButtonDown("Screenshot"))
        {
            ScreenCapture.CaptureScreenshot(screenshotPath);
            Debug.Log("Screenshot saved in \"" + screenshotPath + "\"");

            ++screenshotIndex;
            screenshotPath = Application.dataPath + "/screenshot" + screenshotIndex + ".png";
        }
    }

    void OnApplicationQuit()
    {
        // Stop background threads.
        if (m_backgroundDataProvider != null)
        {
            m_backgroundDataProvider.StopClientThread();
        }
    }

    public void SetFPS(int frameRate)
    {
        this.frameRate = (FPS)frameRate;
    }

    public void SetDepthMode(int depthMode)
    {
        // Add 1 because DepthMode 0 is no depth capture
        this.depthMode = (DepthMode)depthMode + 1;
    }

    public void SetWiredSyncMode(int wiredSyncMode)
    {
        this.wiredSyncMode = (WiredSyncMode)wiredSyncMode;
    }
}
