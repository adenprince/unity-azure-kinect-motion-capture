using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputFileNameInput : MonoBehaviour
{
    public DataRecorder dataRecorder;

    // Start is called before the first frame update
    void Start()
    {
        dataRecorder.setDefaultFileName();
        GetComponent<InputField>().text = dataRecorder.getFileName();
    }
}
