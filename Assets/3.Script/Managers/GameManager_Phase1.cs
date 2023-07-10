using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using TMPro;

public class GameManager_Phase1 : MonoBehaviour
{
    [SerializeField] private Transform[] bossFields;
    [SerializeField] private TextMeshProUGUI text_SharedLife;
    private PhotonView PV;

    private void Awake()
    {
        NetworkManager.instance.onUpdateRoom_SharedLife = UpdateSharedLife;
        TryGetComponent(out PV);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!NetworkManager.instance.TryGetSelectedBoss(DBManager.instance.info.id, out int selectedBossNum))
        {
            return;
        }

        Transform boss = bossFields[selectedBossNum - 1];

        PhotonView pv = PhotonNetwork.Instantiate("Player", boss.position, Quaternion.identity).GetComponent<PhotonView>();
        //pv.transform.SetParent(boss);
        //pv.transform.localPosition = Vector3.zero;

        if (pv != null)
        {
            NetworkManager.instance.PV = pv;
        }
        else
        {
            Debug.Log($"플레이어 생성 후 네트워크에 PhotonView 할당 실패");
        }

        //if (DBManager.instance.info.id.Equals(NetworkManager.instance.GetMasterClient(bossNum)))
        if (NetworkManager.instance.CheckThisIsMaster())
        {
            GameObject yellow = PhotonNetwork.Instantiate("TerrorBringer_Yellow", Vector3.zero, Quaternion.identity);
            PV.RPC("SetTransform_Boss", RpcTarget.AllBuffered, new object[] { yellow.GetComponent<PhotonView>().ViewID, (int)Weather.Spring });

            GameObject green = PhotonNetwork.Instantiate("NightMare_Green", Vector3.zero, Quaternion.identity);
            PV.RPC("SetTransform_Boss", RpcTarget.AllBuffered, new object[] { green.GetComponent<PhotonView>().ViewID, (int)Weather.Summer });

            GameObject purple = PhotonNetwork.Instantiate("Usurper_Purple", Vector3.zero, Quaternion.identity);
            PV.RPC("SetTransform_Boss", RpcTarget.AllBuffered, new object[] { purple.GetComponent<PhotonView>().ViewID, (int)Weather.Autumn });
        }
    }

    [PunRPC]
    private void SetTransform_Boss(int viewID, int weather)
    {
        Transform field = bossFields[weather];
        Transform boss = PhotonView.Find(viewID).transform;
        boss.SetParent(field);
        boss.localPosition = new Vector3(0, 0, 5);
    }

    public void UpdateSharedLife()
    {
        text_SharedLife.text = NetworkManager.instance.GetSharedLife().ToString();
    }
    
}
