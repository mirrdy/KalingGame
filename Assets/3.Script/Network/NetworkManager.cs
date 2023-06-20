using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	string networkState;
	public string BackupRoomName
	{
		get => PlayerPrefs.GetString("BackupRoomName", "");
		set => PlayerPrefs.SetString("BackupRoomName", value);
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
			// ���� ����ų� �����ϴ� ����
			PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions { MaxPlayers = 4, EmptyRoomTtl = 10000 }, null);
		}
		else
			PhotonNetwork.JoinRoom(BackupRoomName);
	}

	public override void OnJoinedRoom()
	{
		BackupRoomName = PhotonNetwork.CurrentRoom.Name;
		PhotonNetwork.Instantiate("Cube", Vector2.zero, Quaternion.identity);
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
}