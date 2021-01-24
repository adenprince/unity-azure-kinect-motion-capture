using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMessages : MonoBehaviour
{
    public GameObject userMessage;
    public float messageTime;
    List<GameObject> userMessages;
    Queue<string> messageQueue;

    void Start()
    {
        userMessages = new List<GameObject>();
        messageQueue = new Queue<string>();
    }

    void Update()
    {
        while (messageQueue.Count != 0)
        {
            StartCoroutine("newUserMessageCR", messageQueue.Dequeue());
        }
    }

    public void newUserMessage(string message)
    {
        StartCoroutine("newUserMessageCR", message);
    }

    // Used to add messages from threads other than the main thread
    public void queueMessage(string message)
    {
        messageQueue.Enqueue(message);
    }

    IEnumerator newUserMessageCR(string message)
    {
        // Move previous messages upward
        foreach (GameObject userMessage in userMessages)
        {
            userMessage.transform.Translate(new Vector3(0.0f, 30.0f, 0.0f));
        }

        // Create new message text with passed message
        GameObject newUserMessage = Instantiate(userMessage, transform);
        newUserMessage.GetComponent<Text>().text = message;
        userMessages.Add(newUserMessage);

        yield return new WaitForSecondsRealtime(messageTime);

        // Delete current message GameObject
        userMessages.RemoveAt(0);
        Destroy(newUserMessage);
    }
}
