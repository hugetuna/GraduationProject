using UnityEngine;

public class TMPWarningFilter : MonoBehaviour
{
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 過濾掉 TMP 字型缺字的警告
        if (type == LogType.Warning &&
            (logString.Contains("Underline is not available in font asset")|| logString.Contains("The character with Unicode value")))
        {
            return; // 忽略，不顯示在 Console
        }

        // 其他訊息照常顯示
        Debug.unityLogger.Log(type, logString);
    }
}
