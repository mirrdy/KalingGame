using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyLogManager : MonoBehaviour
{
    public static LobbyLogManager instance;
    public ScrollRect scrollRect;
    public TextMeshProUGUI text_Log;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        text_Log.text = string.Empty;
    }

    public void AddLog(string logMessage)
    {
        text_Log.text += logMessage + "\n";
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }
}
