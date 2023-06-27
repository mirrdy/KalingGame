using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyUIManager : MonoBehaviour
{
    public static LobbyUIManager instance;
    public ScrollRect scrollRect;
    public TextMeshProUGUI text_Log;
    public TextMeshProUGUI text_PlayerCount;
    [SerializeField] private TextMeshProUGUI[] text_BossSelCounts;
    
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

        text_Log.text = string.Empty;
        // 클라이언트 접속마다 대리자에 중복해서 추가되지 않도록 하나씩만 할당
        NetworkManager.instance.onJoinedRoomDele = DisplayRoomInfo;
        NetworkManager.instance.onUpdateRoomProperty = DisplayRoomInfo;
    }
    
    public void AddLog(string logMessage)
    {
        text_Log.text += logMessage + "\n";
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    private void DisplayRoomInfo()
    {
        Room room = PhotonNetwork.CurrentRoom;
        if (room != null)
        {
            text_PlayerCount.text = $"입장인원: ({room.PlayerCount} / {room.MaxPlayers})";

            // 보스 선택 인원 표시
            for (int i = 0; i < text_BossSelCounts.Length; i++)
            {
                int bossSelCount = NetworkManager.instance.GetBossSelectionCount(i+1);
                if (bossSelCount < 0)
                {
                    bossSelCount = 0;
                }
                text_BossSelCounts[i].text = $"선택 인원: {bossSelCount}";
            }
        }
    }
}
