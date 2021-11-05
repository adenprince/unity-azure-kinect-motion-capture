using UnityEngine;

public class TwoSensorsRecorderSetup : MonoBehaviour
{
    public GameObject main;
    public GameObject tracker;
    
    void Awake()
    {
        main.GetComponent<main>().SetTwoSensors(true);
        tracker.GetComponent<TrackerHandler>().SetTwoSensors(true);
        GetComponent<DataRecorder>().setWriteData(true);
        GetComponent<DataRecorder>().setFileName("mergedOutput.csv");
        Time.timeScale = 1f;
    }
}
