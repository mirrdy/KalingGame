using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
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

		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		Player[] sortedPlayers = PhotonNetwork.PlayerList;

		for (int i = 0; i < sortedPlayers.Length; i++)
		{
			if (sortedPlayers[i].ActorNumber == actorNumber)
			{
				playerID = i;
				break;
			}
		}

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

	public void OnApplicationQuit()
	{
		BackupRoomName = "";
	}
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
					// Add custom properties as needed
					{"selection_Boss1", 0 },
					{"selection_Boss2", 0 },
					{"selection_Boss3", 0 },
				};
			room.SetCustomProperties(customProperties);
		}
	}
}