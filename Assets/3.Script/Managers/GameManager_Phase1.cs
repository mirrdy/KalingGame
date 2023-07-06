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
        PhotonView pv = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity).GetComponentInChildren<PhotonView>();
        if (pv != null)
        {
            NetworkManager.instance.PV = pv;
        }
        else
        {
            Debug.Log($"�÷��̾� ���� �� ��Ʈ��ũ�� PhotonView �Ҵ� ����");
        }
        // 1������ �� -> �����ѹ� 1
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
