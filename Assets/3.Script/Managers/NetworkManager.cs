using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	public static NetworkManager instance = null;

	string networkState;

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
			print(networkState);
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
			PhotonNetwork.JoinRoom(BackupRoomName);
	}

	public override void OnJoinedRoom()
	{
		BackupRoomName = PhotonNetwork.CurrentRoom.Name;

		// 커스텀 프로퍼티
		Room room = PhotonNetwork.CurrentRoom;
		ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable
		{
			// Add custom properties as needed
			{"selection_Boss1", 0 },
			{"selection_Boss2", 0 },
			{"selection_Boss3", 0 }
		};
		room.SetCustomProperties(customProperties);
		//PhotonNetwork.Instantiate("Cube", Vector2.zero, Quaternion.identity);
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
}