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

        // Ŭ���̾�Ʈ ���Ӹ��� �븮�ڿ� �ߺ��ؼ� �߰����� �ʵ��� �ϳ����� �Ҵ�
        NetworkManager.instance.onJoinedRoomDele = DisplayRoomInfo;
        NetworkManager.instance.onUpdateRoom_PlayerID = DisplayRoomInfo;
        NetworkManager.instance.onUpdateRoom_IsStartedGame = StartGame;

        // LobbyUIManager�� awake �ϱ� ���� �̹� RoomJoin �̺�Ʈ�� �߻����� - Ŭ���̾�Ʈ���� �� ��ȯ �� �ѹ����� ���� UI ������Ʈ
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
            text_PlayerCount.text = $"�����ο�: ({room.PlayerCount} / {room.MaxPlayers})";

            
            // ���� ���� �ο� ǥ��
            for (int i = 0; i < text_BossSelCounts.Length; i++)
            {
                int bossSelCount = NetworkManager.instance.GetBossSelectionCount(i+1);
                if (bossSelCount <= 0)
                {
                    bossSelCount = 0;
                    isReadyForStart = false;
                }
                totalSelCount += bossSelCount;
                text_BossSelCounts[i].text = $"���� �ο�: {bossSelCount}";
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
