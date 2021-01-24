using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMessages : MonoBehaviour
{
    public GameObject userMessage;
    List<GameObject> userMessages = new List<GameObject>();

    public void newUserMessage(string message)
    {
        StartCoroutine("newUserMessageCR", message);
    }

    IEnumerator newUserMessageCR(string message)
    {
        // Move previous messages upward
        foreach (GameObject userMessage in userMessages) {
            userMessage.transform.Translate(new Vector3(0.0f, 20.0f, 0.0f));
        }

        // Create new message text with passed message
        GameObject newUserMessage = Instantiate(userMessage, transform);
        newUserMessage.GetComponent<Text>().text = message;
        userMessages.Add(newUserMessage);

        yield return new WaitForSecondsRealtime(3.0f);

        // Delete current message GameObject
        userMessages.RemoveAt(0);
        Destroy(newUserMessage);
    }
}
