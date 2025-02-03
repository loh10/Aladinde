using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayConsole : MonoBehaviour
{
    public Text consoleText;

    void Start()
    {
        if (Application.platform != RuntimePlatform.WindowsServer)
        {
            gameObject.AddComponent<ConsoleLogger>().consoleText = consoleText;
        }
    }
}
public class ConsoleLogger : MonoBehaviour
{
    private static Queue<string> logMessages = new Queue<string>();
    public Text consoleText;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (logMessages.Count >= 10)
        {
            logMessages.Dequeue();
        }
        logMessages.Enqueue(logString);
        UpdateConsoleText();
    }

    void UpdateConsoleText()
    {
        consoleText.text = string.Join("\n", logMessages.ToArray());
    }
}
