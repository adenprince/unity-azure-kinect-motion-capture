using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;

public class main : MonoBehaviour
{
    // Handler for SkeletalTracking thread.
    public GameObject m_tracker;
    private SkeletalTrackingProvider m_skeletalTrackingProvider;
    private SkeletalTrackingProvider m_skeletalTrackingProvider2;
    public BackgroundData m_lastFrameData = new BackgroundData();
    public BackgroundData m_lastFrameData2 = new BackgroundData();

    public ColorCameraView colorCameraView;
    public ColorCameraView colorCameraView2;

    public FPS frameRate = FPS.FPS30;
    public DepthMode depthMode = DepthMode.NFOV_Unbinned;

    public UserMessages userMessages;

    bool twoSensors = false;

    void Start()
    {
        //tracker ids needed for when there are two trackers
        const int TRACKER_ID = 0;
        m_skeletalTrackingProvider = new SkeletalTrackingProvider(TRACKER_ID, colorCameraView, frameRate,
            depthMode, userMessages);

        if (twoSensors)
        {
            const int TRACKER_ID_2 = 1;
            m_skeletalTrackingProvider2 = new SkeletalTrackingProvider(TRACKER_ID_2, colorCameraView2, frameRate,
                depthMode, userMessages);
        }
    }

    void Update()
    {
        if (twoSensors)
        {
            if (m_skeletalTrackingProvider.IsRunning && m_skeletalTrackingProvider2.IsRunning &&
                m_skeletalTrackingProvider.GetCurrentFrameData(ref m_lastFrameData) && m_skeletalTrackingProvider2.GetCurrentFrameData(ref m_lastFrameData2))
            {
                m_tracker.GetComponent<TrackerHandler>().updateTrackerTwoSensors(m_lastFrameData, m_lastFrameData2);
            }
        }
        else
        {
            if (m_skeletalTrackingProvider.GetCurrentFrameData(ref m_lastFrameData))
            {
                m_tracker.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData);
            }
        }
    }

    void OnApplicationQuit()
    {
        if (m_skeletalTrackingProvider != null)
        {
            m_skeletalTrackingProvider.Dispose();
        }

        if (m_skeletalTrackingProvider2 != null)
        {
            m_skeletalTrackingProvider2.Dispose();
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

    public void SetTwoSensors(bool twoSensors)
    {
        this.twoSensors = twoSensors;
    }
}
