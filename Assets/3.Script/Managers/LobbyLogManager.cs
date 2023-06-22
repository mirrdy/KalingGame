using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyLogManager : MonoBehaviour
{
    public static LobbyLogManager instance;
    public ScrollRect scrollRect;
    public TextMeshProUGUI text_Log;
    public TextMeshProUGUI text_PlayerCount;
    [SerializeField] private TextMeshProUGUI text_Count_Boss1;
    [SerializeField] private TextMeshProUGUI text_Count_Boss2;
    [SerializeField] private TextMeshProUGUI text_Count_Boss3;

    private int oldPlayerCount;
    
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
        // 클라이언트 접속마다 대리자에 중복해서 추가되지 않도록 하나씩만 할당
        NetworkManager.instance.onUpdateRoomProperty = DisplayRoomInfo;
    }
    private void Update()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            if (oldPlayerCount != PhotonNetwork.CurrentRoom.PlayerCount)
            {
                DisplayPlayerCount();
            }
        }

    }
    public void AddLog(string logMessage)
    {
        text_Log.text += logMessage + "\n";
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }
    private void DisplayPlayerCount()
    {
        
    }
    private void DisplayRoomInfo()
    {
        Room room = PhotonNetwork.CurrentRoom;
        if (room != null)
        {
            text_PlayerCount.text = $"입장인원: ({room.PlayerCount} / {room.MaxPlayers})";

            ExitGames.Client.Photon.Hashtable customProperties = room.CustomProperties;

            int selectionBoss1 = (int)customProperties["selection_Boss1"];
            int selectionBoss2 = (int)customProperties["selection_Boss2"];
            int selectionBoss3 = (int)customProperties["selection_Boss3"];

            text_Count_Boss1.text = $"선택 인원: {selectionBoss1}";
            text_Count_Boss2.text = $"선택 인원: {selectionBoss2}";
            text_Count_Boss3.text = $"선택 인원: {selectionBoss3}";
        }
    }
}
