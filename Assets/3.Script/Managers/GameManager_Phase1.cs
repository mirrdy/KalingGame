using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;


public class GameManager_Phase1 : MonoBehaviour
{
    [SerializeField] private int bossNum = 1;
    // Start is called before the first frame update
    void Start()
    {
        // 1페이즈 씬 -> 보스넘버 1
        if (DBManager.instance.info.id.Equals(NetworkManager.instance.GetMasterClient(bossNum)))
        {
            PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate("NightMare_Green", new Vector3(0, 0, 5), Quaternion.identity);
            PhotonNetwork.Instantiate("LineSpawner", Vector3.zero, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
