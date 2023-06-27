using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Linq;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	private int playerCount = 0;
	public int playerID = -1;
	public static NetworkManager instance = null;
	string networkState;

	public delegate void OnJoinedRoomDele();
	public OnJoinedRoomDele onJoinedRoomDele;
	public delegate void OnUpdateRoomProperty();
	public OnUpdateRoomProperty onUpdateRoomProperty;

	public string BackupRoomName
	{
		get => PlayerPrefs.GetString("BackupRoomName", "");
		set => PlayerPrefs.SetString("BackupRoomName", value);
	}

    private void Awake()
    {
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}
    void Start()
	{
		PhotonNetwork.ConnectUsingSettings();
	}

    void Update()
    {
        string curNetworkState = PhotonNetwork.NetworkClientState.ToString();
        if (networkState != curNetworkState)
        {
            networkState = curNetworkState;
        }

        // 접속이 종료된 클라이언트가 발생
        // 접속 종료시 안정적인 이벤트 처리 방법이 떠오르면 수정할 예정
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            if (playerCount > PhotonNetwork.CurrentRoom.PlayerCount)
            {
                playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

                Room room = PhotonNetwork.CurrentRoom;
                Dictionary<int, string> dict_ID = room.CustomProperties["player_ID"] as Dictionary<int, string>;

                foreach (var key in dict_ID.Keys)
                {
                    if (!room.Players.Keys.Contains(key))
                    {
                        dict_ID.Remove(key);
                    }
                }

                ExitGames.Client.Photon.Hashtable customProperties =
                    new ExitGames.Client.Photon.Hashtable
                    {
                    { "player_ID", dict_ID },
                    };
                room.SetCustomProperties(customProperties);
            }
        }
    }

    public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		if (BackupRoomName == "")
		{
			// 방을 만들거나 참여하는 로직
			PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions { MaxPlayers = 3, EmptyRoomTtl = 10000 }, null);
		}
		else
		{
			PhotonNetwork.JoinRoom(BackupRoomName);
		}
	}
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

		// 커스텀 프로퍼티 - Room을 Create를 해도 OnCreateRoom 이벤트보다 OnJoinedRoom이 먼저 실행되는 현상 발생
		InitCustomProperty();
	}
    public override void OnJoinedRoom()
	{
		BackupRoomName = PhotonNetwork.CurrentRoom.Name;

		/*int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		Player[] sortedPlayers = PhotonNetwork.PlayerList;

		for (int i = 0; i < sortedPlayers.Length; i++)
		{
			if (sortedPlayers[i].ActorNumber == actorNumber)
			{
				playerID = i;
				break;
			}
		}*/

		InitCustomProperty();
		onJoinedRoomDele?.Invoke();
	}

	public override void OnDisconnected(DisconnectCause cause) 
	{
        PhotonNetwork.Reconnect();
    }

	public override void OnLeftRoom()
	{
		BackupRoomName = "";
	}
    
	// Quit 콜백 내부에서 room 프로퍼티를 변경하고 네트워크에 동기화되기 전에
	// 프로그램이 종료되어서 프로퍼티가 적용 안되는 것 같음
    /*private void OnApplicationQuit()
    {
		if(PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
			Debug.Log("아직 방에 있음, 준비됨");
        }
		BackupRoomName = "";
		Room room = PhotonNetwork.CurrentRoom;
		string id = (room.CustomProperties["player_ID"] as Dictionary<int, string>)[PhotonNetwork.LocalPlayer.ActorNumber];

		UpdatePlayerSelectionData(id, -1);

		// Quit 이벤트 이후엔 프로퍼티 업데이트 이벤트가 호출되지 않는 것 같음 => 그냥 호출 안됨.
		// 명시적으로 대리자 호출
		onUpdateRoomProperty.Invoke();
	}*/

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		onUpdateRoomProperty.Invoke();
	}
	private void InitCustomProperty()
    {
		Room room = PhotonNetwork.CurrentRoom;
		if (room.CustomProperties.Count <= 0)
		{
			ExitGames.Client.Photon.Hashtable customProperties =
				new ExitGames.Client.Photon.Hashtable
				{
					// 커스텀 프로퍼티 만들기
					{"player_ID", new Dictionary<int, string>()},
					{"player_SelectionData", new Dictionary<string, int>()}
				};
			room.SetCustomProperties(customProperties);
		}
	}
	public void UpdatePlayerSelectionData(string id, int sel)
    {
		Room room = PhotonNetwork.CurrentRoom;
        if (!(room.CustomProperties["player_SelectionData"] is Dictionary<string, int> dict_SelNum) ||
			!(room.CustomProperties["player_ID"] is Dictionary<int, string> dict_ID))
        {
            return;
        }

		// 보스 최초 선택
        if (!dict_SelNum.ContainsKey(id))
        {
			dict_ID.Add(PhotonNetwork.LocalPlayer.ActorNumber, id);
			dict_SelNum.Add(id, sel);
        }
		else
        {
			foreach(var key in dict_ID.Keys)
            {
				if(dict_ID[key] == id)
                {
					dict_ID.Remove(key);
					dict_ID.Add(PhotonNetwork.LocalPlayer.ActorNumber, id);
					break;
				}
            }
			
			dict_SelNum[id] = sel;
        }
		// Dictionary가 참조형이라 selDict만 변경해도 프로퍼티가 변경되지만
		// 메서드로 업데이트 하지 않으면 프로퍼티 업데이트 이벤트가 작동하지 않는 것 같음
		ExitGames.Client.Photon.Hashtable customProperties =
                new ExitGames.Client.Photon.Hashtable
                {
					{ "player_ID", dict_ID },
                    { "player_SelectionData", dict_SelNum }
                };
        room.SetCustomProperties(customProperties);
    }
    public int GetBossSelectionCount(int bossNum)
    {
		Room room = PhotonNetwork.CurrentRoom;
        if (!(room.CustomProperties["player_SelectionData"] is Dictionary<string, int> selDict))
        {
            return -1;
        }

		int count = 0;

		foreach (KeyValuePair<string, int> entry in selDict)
		{
			if(entry.Value == bossNum)
            {
				count++;
            }
		}
		return count;
	}
}