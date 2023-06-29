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

	private List<RoomInfo> list_Room = new List<RoomInfo>();
	private int roomNum = 0;
	// Custom Property Name
	public readonly string prop_PlayerID = "player_ID";
	public readonly string prop_PlayerSelectionData = "player_SelectionData";

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

        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
			// ������ ���� �ο� ���� �߻�
			if(playerCount != PhotonNetwork.CurrentRoom.PlayerCount)
            {
				// ������ ����� Ŭ���̾�Ʈ�� �߻�
				// ���� ����� �������� �̺�Ʈ ó�� ����� �������� ������ ����
				if (playerCount > PhotonNetwork.CurrentRoom.PlayerCount)
				{
					Room room = PhotonNetwork.CurrentRoom;

					Dictionary<int, string> dict_ID = room.CustomProperties[prop_PlayerID] as Dictionary<int, string>;
					Dictionary<string, int> dict_Sel = room.CustomProperties[prop_PlayerSelectionData] as Dictionary<string, int>;

					// Enumerable ��ü�� ��ȸ�ϸ鼭 �����ϸ� �ȵǴϱ� key�� ���� ���� �� ó����
					List<int> keysForRemove = new List<int>();
					foreach (int roomID in dict_ID.Keys)
					{
						if (!room.Players.Keys.Contains(roomID))
						{
							dict_Sel[dict_ID[roomID]] = -1;
							keysForRemove.Add(roomID);
						}
					}
					foreach(int roomID in keysForRemove)
                    {
						dict_ID.Remove(roomID);
					}
					

                    ExitGames.Client.Photon.Hashtable customProperties =
                        new ExitGames.Client.Photon.Hashtable
                        {
							{ prop_PlayerID, dict_ID },
							{ prop_PlayerSelectionData, dict_Sel}
                        };
                    room.SetCustomProperties(customProperties);
				}

				// �ο� ������ �ʿ��� ���� �� �ο��� ����ȭ
                playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            }
        }
    }

    public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{

    }
    public void JoinRoom()
    {
        // ���� ���� �� �ɼ� ����

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 3,
            EmptyRoomTtl = 10000,

            // Your properties
        };

        ExitGames.Client.Photon.Hashtable customProperties =
                new ExitGames.Client.Photon.Hashtable
                {
					// Ŀ���� ������Ƽ �����
					{prop_PlayerID, new Dictionary<int, string>()},
					{prop_PlayerSelectionData, new Dictionary<string, int>()}
                };

		roomOptions.CustomRoomProperties = customProperties;
		roomOptions.CustomRoomPropertiesForLobby = new string[] { prop_PlayerID, prop_PlayerSelectionData };

		PhotonNetwork.JoinOrCreateRoom($"room{roomNum}", roomOptions, null);
	}

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
		roomNum++;
		JoinRoom();
	}
    // �� ���� ���н� �ݹ�
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
		roomNum++;
		JoinRoom();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

		// Ŀ���� ������Ƽ - Room�� Create�� �ص� OnCreateRoom �̺�Ʈ���� OnJoinedRoom�� ���� ����Ǵ� ���� �߻�
		InitCustomProperty();
	}

	// ���� ������ ��
    public override void OnJoinedRoom()
	{
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

	// �ٸ� �÷��̾ ������ ��
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
		onJoinedRoomDele?.Invoke();
	}

	public override void OnDisconnected(DisconnectCause cause) 
	{
        PhotonNetwork.Reconnect();
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
		string id = (room.CustomProperties[prop_PlayerID] as Dictionary<int, string>)[PhotonNetwork.LocalPlayer.ActorNumber];

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
		
		
	}
	public void UpdatePlayerSelectionData(string id, int sel)
    {
        Room room = PhotonNetwork.CurrentRoom;
        if (!(room.CustomProperties[prop_PlayerID] is Dictionary<int, string> dict_ID) ||
            !(room.CustomProperties[prop_PlayerSelectionData] is Dictionary<string, int> dict_SelNum))
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
					{ prop_PlayerID, dict_ID },
                    { prop_PlayerSelectionData, dict_SelNum }
                };
        room.SetCustomProperties(customProperties);
    }
    public int GetBossSelectionCount(int bossNum)
    {
		Room room = PhotonNetwork.CurrentRoom;
        if (!(room.CustomProperties[prop_PlayerSelectionData] is Dictionary<string, int> dict_Sel))
        {
            return -1;
        }

		int count = 0;

		foreach (KeyValuePair<string, int> pair_Sel in dict_Sel)
		{
			if(pair_Sel.Value == bossNum)
            {
				count++;
            }
		}
        return count;
    }
	public bool TryGetSelectedBoss(string id, out int selectedBossNum)
    {
		bool isSuccess = false;
		selectedBossNum = -1;

		Room room = PhotonNetwork.CurrentRoom;
		if (!(room.CustomProperties[prop_PlayerSelectionData] is Dictionary<string, int> dict_SelNum))
		{
			return false;
		}

		if(dict_SelNum.TryGetValue(id, out selectedBossNum))
        {
			isSuccess = true;
        }

		return isSuccess;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // �� ����Ʈ �ݹ��� �κ� ���������� �ڵ����� ȣ��ȴ�.
        // �κ񿡼��� ȣ�� ����
        list_Room = roomList;
    }
}