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

        // ������ ����� Ŭ���̾�Ʈ�� �߻�
        // ���� ����� �������� �̺�Ʈ ó�� ����� �������� ������ ����
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
			// ���� ����ų� �����ϴ� ����
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

		// Ŀ���� ������Ƽ - Room�� Create�� �ص� OnCreateRoom �̺�Ʈ���� OnJoinedRoom�� ���� ����Ǵ� ���� �߻�
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
    
	// Quit �ݹ� ���ο��� room ������Ƽ�� �����ϰ� ��Ʈ��ũ�� ����ȭ�Ǳ� ����
	// ���α׷��� ����Ǿ ������Ƽ�� ���� �ȵǴ� �� ����
    /*private void OnApplicationQuit()
    {
		if(PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
			Debug.Log("���� �濡 ����, �غ��");
        }
		BackupRoomName = "";
		Room room = PhotonNetwork.CurrentRoom;
		string id = (room.CustomProperties["player_ID"] as Dictionary<int, string>)[PhotonNetwork.LocalPlayer.ActorNumber];

		UpdatePlayerSelectionData(id, -1);

		// Quit �̺�Ʈ ���Ŀ� ������Ƽ ������Ʈ �̺�Ʈ�� ȣ����� �ʴ� �� ���� => �׳� ȣ�� �ȵ�.
		// ��������� �븮�� ȣ��
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
					// Ŀ���� ������Ƽ �����
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

		// ���� ���� ����
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
		// Dictionary�� �������̶� selDict�� �����ص� ������Ƽ�� ���������
		// �޼���� ������Ʈ ���� ������ ������Ƽ ������Ʈ �̺�Ʈ�� �۵����� �ʴ� �� ����
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