using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;

public class main : MonoBehaviour
{
    // Handler for SkeletalTracking thread.
    public GameObject m_tracker;
    private SkeletalTrackingProvider m_skeletalTrackingProvider;
    public BackgroundData m_lastFrameData = new BackgroundData();

    public ColorCameraView colorCameraView;

    public FPS frameRate = FPS.FPS30;
    public DepthMode depthMode = DepthMode.NFOV_Unbinned;
    public WiredSyncMode wiredSyncMode = WiredSyncMode.Standalone;

    public UserMessages userMessages;
    
    void Start()
    {
        //tracker ids needed for when there are two trackers
        const int TRACKER_ID = 0;
        m_skeletalTrackingProvider = new SkeletalTrackingProvider(TRACKER_ID, colorCameraView, frameRate,
            depthMode, wiredSyncMode, userMessages);
    }

    void Update()
    {
        if (m_skeletalTrackingProvider.IsRunning)
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
