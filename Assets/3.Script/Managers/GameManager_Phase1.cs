using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using TMPro;

public class GameManager_Phase1 : MonoBehaviour
{
    [SerializeField] private int bossNum = 1;
    [SerializeField] private TextMeshProUGUI text_SharedLife;


    private void Awake()
    {
        NetworkManager.instance.onUpdateRoom_SharedLife = UpdateSharedLife;
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        // 1페이즈 씬 -> 보스넘버 1
        if (DBManager.instance.info.id.Equals(NetworkManager.instance.GetMasterClient(bossNum)))
        {
            PhotonNetwork.Instantiate("NightMare_Green", new Vector3(0, 0, 5), Quaternion.identity);
            PhotonNetwork.Instantiate("LineSpawner", Vector3.zero, Quaternion.identity);
        }
    }

    public void UpdateSharedLife()
    {
        text_SharedLife.text = NetworkManager.instance.GetSharedLife().ToString();
    }
}
