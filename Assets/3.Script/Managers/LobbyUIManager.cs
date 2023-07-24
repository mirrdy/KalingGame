using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyUIManager : MonoBehaviour
{
    public static LobbyUIManager instance;
    [SerializeField] private TextMeshProUGUI text_PlayerID;
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

        // 클라이언트 접속마다 대리자에 중복해서 추가되지 않도록 하나씩만 할당
        NetworkManager.instance.onJoinedRoomDele = DisplayRoomInfo;
        NetworkManager.instance.onUpdateRoom_PlayerID = DisplayRoomInfo;
        NetworkManager.instance.onUpdateRoom_IsStartedGame = StartGame;

        // LobbyUIManager가 awake 하기 전에 이미 RoomJoin 이벤트가 발생했음 - 클라이언트에서 씬 전환 후 한번씩은 수동 UI 업데이트
        DisplayRoomInfo();
        DisplayPlayerID();
    }

    private void DisplayRoomInfo()
    {
        bool isReadyForStart = true;
        Room room = PhotonNetwork.CurrentRoom;
        int totalSelCount = 0;
        if (room != null)
        {
            text_PlayerCount.text = $"입장인원: ({room.PlayerCount} / {room.MaxPlayers})";

            
            // 보스 선택 인원 표시
            for (int i = 0; i < text_BossSelCounts.Length; i++)
            {
                int bossSelCount = NetworkManager.instance.GetBossSelectionCount(i+1);
                if (bossSelCount <= 0)
                {
                    bossSelCount = 0;
                    isReadyForStart = false;
                }
                totalSelCount += bossSelCount;
                text_BossSelCounts[i].text = $"선택 인원: {bossSelCount}";
            }
        }

        if(totalSelCount >= 3)
        {
            NetworkManager.instance.StartBossGame();
        }
        //if(isReadyForStart)
        //{
        //    NetworkManager.instance.StartBossGame();
        //}   
    }
    private void DisplayPlayerID()
    {
        text_PlayerID.text = DBManager.instance.info.id;
    }
    private void StartGame()
    {
        NetworkManager.instance.StartBossGame();
    }
}
