using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string newSceneName;

    public void switchScene()
    {
        SceneManager.LoadScene(newSceneName, LoadSceneMode.Single);
    }
}
