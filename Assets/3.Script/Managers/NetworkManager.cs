using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading.Tasks;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	public List<GameObject> Prefabs;

	private int playerCount = 0;
	public int playerID = -1;
	public static NetworkManager instance = null;
	string networkState;

	public delegate void OnJoinedRoomDele();
	public OnJoinedRoomDele onJoinedRoomDele;
	public delegate void OnUpdateRoomProperty();
	public OnUpdateRoomProperty onUpdateRoomProperty;

	private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
	private List<RoomInfo> list_Room = new List<RoomInfo>();
	private int roomNum = 0;
	// Custom Property Name
	public readonly string prop_PlayerID = "player_ID";
	public readonly string prop_PlayerSelectionData = "player_SelectionData";
	public readonly string prop_CanJoin = "canJoin";
	public readonly string prop_MasterClientID = "masterClientID";
	private readonly string MethodExecutedKey = "MethodExecuted";

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
        if (PhotonNetwork.PrefabPool is DefaultPool pool && this.Prefabs != null)
        {
            foreach (GameObject prefab in this.Prefabs)
            {
                pool.ResourceCache.Add(prefab.name, prefab);
            }
        }

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
			// 서버에 접속 인원 변동 발생
			if(playerCount != PhotonNetwork.CurrentRoom.PlayerCount)
            {
				// 접속이 종료된 클라이언트가 발생
				// 접속 종료시 안정적인 이벤트 처리 방법이 떠오르면 수정할 예정
				if (playerCount > PhotonNetwork.CurrentRoom.PlayerCount)
				{
					Room room = PhotonNetwork.CurrentRoom;

					Dictionary<int, string> dict_ID = room.CustomProperties[prop_PlayerID] as Dictionary<int, string>;
					Dictionary<string, int> dict_Sel = room.CustomProperties[prop_PlayerSelectionData] as Dictionary<string, int>;

					// Enumerable 객체를 순회하면서 변조하면 안되니까 key를 따로 저장 후 처리함
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

				// 인원 변동시 필요한 동작 후 인원수 동기화
                playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            }
        }
    }

	private void UpdateCachedRoomList(List<RoomInfo> roomList)
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			RoomInfo info = roomList[i];
			if (info.RemovedFromList)
			{
				cachedRoomList.Remove(info.Name);
			}
			else
			{
				cachedRoomList[info.Name] = info;
			}
		}
	}
	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
	}
	public override void OnLeftLobby()
	{
		cachedRoomList.Clear();
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		cachedRoomList.Clear();
	}

	public override void OnJoinedLobby()
	{
		cachedRoomList.Clear();
	}
    public void JoinRoom()
    {
        // 방을 만들 때 옵션 설정

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 3,
            EmptyRoomTtl = 10000,

            // Your properties
        };

        ExitGames.Client.Photon.Hashtable customProperties =
                new ExitGames.Client.Photon.Hashtable
                {
					// 커스텀 프로퍼티 만들기
					{prop_PlayerID, new Dictionary<int, string>()},
					{prop_PlayerSelectionData, new Dictionary<string, int>()},
					{prop_MasterClientID, new Dictionary<int, string>() },
					{prop_CanJoin, true },
                };

		roomOptions.CustomRoomProperties = customProperties;

		// 로비에서 볼 수 있는 룸 프로퍼티 설정
		roomOptions.CustomRoomPropertiesForLobby = new string[] { prop_PlayerID, prop_PlayerSelectionData, prop_MasterClientID, prop_CanJoin};

		string roomName = string.Empty;
		foreach(var pair in cachedRoomList)
        {
			if((bool)(pair.Value.CustomProperties[prop_CanJoin]))
            {
				roomName = pair.Key;
				break;
            }
        }

		if (roomName.Equals(string.Empty))
		{
			roomName = $"room{roomNum}";
		}
		PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
	}

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
		roomNum++;
		JoinRoom();
	}
    // 방 생성 실패시 콜백
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
		roomNum++;
		JoinRoom();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

		// 커스텀 프로퍼티 - Room을 Create를 해도 OnCreateRoom 이벤트보다 OnJoinedRoom이 먼저 실행되는 현상 발생
		InitCustomProperty();
	}

	// 내가 들어왔을 때
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

	// 다른 플레이어가 들어왔을 때
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
		onJoinedRoomDele?.Invoke();
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
		string id = (room.CustomProperties[prop_PlayerID] as Dictionary<int, string>)[PhotonNetwork.LocalPlayer.ActorNumber];

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
			
	}
	public void UpdatePlayerSelectionData(string id, int sel)
    {
        Room room = PhotonNetwork.CurrentRoom;
        if (!(room.CustomProperties[prop_PlayerID] is Dictionary<int, string> dict_ID) ||
            !(room.CustomProperties[prop_PlayerSelectionData] is Dictionary<string, int> dict_SelNum))
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
		UpdateCachedRoomList(roomList);
	}
	public void SetUnableJoinRoom()
    {
		Room room = PhotonNetwork.CurrentRoom;

		ExitGames.Client.Photon.Hashtable customProperties =
				new ExitGames.Client.Photon.Hashtable
				{
					{prop_CanJoin, false}
				};

		room.SetCustomProperties(customProperties);
	}

	public async void StartBossGame()
    {
		if (TryGetSelectedBoss(DBManager.instance.info.id, out int selectedBossNum))
		{
			SetUnableJoinRoom();
			SelectMasterClient();
			// 마스터 클라이언트 프로퍼티 업데이트 될 때까지 대기 - UI 안멈추게 태스크 만들어서 대기
			await Task.Run(() =>
			{
				Room room = PhotonNetwork.CurrentRoom;
				while (true)
				{
					if (!(room.CustomProperties[prop_MasterClientID] is Dictionary<int, string> dict_MasterID))
					{
						continue;
					}
					if(dict_MasterID.Count > 0)
                    {
						break;
                    }
				}
			});
			SceneManager.LoadScene($"Phase1Boss{selectedBossNum}");
		}
	}

    private void SelectMasterClient()
    {
		Room room = PhotonNetwork.CurrentRoom;

		if (!(room.CustomProperties[prop_PlayerID] is Dictionary<int, string> dict_ID) ||
			!(room.CustomProperties[prop_PlayerSelectionData] is Dictionary<string, int> dict_SelNum))
		{
			Debug.Log("SelectMasterClient - CurrentRoom's CustomProperty is null");
			return;
		}

		if (!IsMethodExecuted())
		{
            // Execute the method

            // Set the "MethodExecuted" property to true atomically
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
            {
                { MethodExecutedKey, true }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
		}
		

        List<List<string>> list_BossSel = new List<List<string>> { new List<string>(), new List<string>(), new List<string>() };

        foreach (var id in dict_ID.Values)
        {
			// list_BossSel의 n-1번째 인덱스에 보스n을 선택한 유저의 닉네임을 추가함
			list_BossSel[dict_SelNum[id] - 1].Add(id);
        }
		System.Random random = new System.Random();

		Dictionary<int, string> dict_Master = new Dictionary<int, string>();

		for(int i=0; i<list_BossSel.Count; i++)
        {
			if(list_BossSel[i].Count <= 0)
            {
				continue;
            }
			int randomIndex = random.Next(list_BossSel[i].Count);

			dict_Master.Add((i+1), list_BossSel[i][randomIndex]);
        }

		ExitGames.Client.Photon.Hashtable customProperties =
				new ExitGames.Client.Photon.Hashtable
				{
					{prop_MasterClientID, dict_Master}
				};

		room.SetCustomProperties(customProperties);
	}
	public string GetMasterClient(int bossNum)
    {
		Room room = PhotonNetwork.CurrentRoom;

		if (!(room.CustomProperties[prop_MasterClientID] is Dictionary<int, string> dict_MasterID))
		{
			Debug.Log("GetMasterClient - CurrentRoom's CustomProperty is null");
			return string.Empty;
		}

		return dict_MasterID[bossNum];
	}

	private bool IsMethodExecuted()
	{
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MethodExecutedKey, out object methodExecutedObj))
		{
			return (bool)methodExecutedObj;
		}
		return false;
	}
}