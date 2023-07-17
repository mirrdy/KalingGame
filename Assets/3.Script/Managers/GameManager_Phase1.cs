using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using TMPro;

public class GameManager_Phase1 : MonoBehaviour
{
    [SerializeField] private Transform[] bossFields;
    private PhotonView PV;

    private void Awake()
    {
        TryGetComponent(out PV);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!NetworkManager.instance.TryGetSelectedBoss(DBManager.instance.info.id, out int selectedBossNum))
        {
            return;
        }

        Transform bossField = bossFields[selectedBossNum - 1];

        PhotonView pv = PhotonNetwork.Instantiate("Player", bossField.position, Quaternion.identity).GetComponent<PhotonView>();
        //pv.transform.SetParent(boss);
        //pv.transform.localPosition = Vector3.zero;

        if (pv != null)
        {
            NetworkManager.instance.PV = pv;
        }
        else
        {
            Debug.Log($"�÷��̾� ���� �� ��Ʈ��ũ�� PhotonView �Ҵ� ����");
        }

        //if (DBManager.instance.info.id.Equals(NetworkManager.instance.GetMasterClient(bossNum)))
        if (NetworkManager.instance.CheckThisIsMaster())
        {
            GameObject yellow = PhotonNetwork.Instantiate("TerrorBringer_Yellow", Vector3.zero, Quaternion.identity);
            PV.RPC("SetTransform_Boss", RpcTarget.AllBuffered, new object[] { yellow.GetComponent<PhotonView>().ViewID, (int)Season.Spring });

            GameObject green = PhotonNetwork.Instantiate("NightMare_Green", Vector3.zero, Quaternion.identity);
            PV.RPC("SetTransform_Boss", RpcTarget.AllBuffered, new object[] { green.GetComponent<PhotonView>().ViewID, (int)Season.Summer });

            GameObject purple = PhotonNetwork.Instantiate("Usurper_Purple", Vector3.zero, Quaternion.identity);
            PV.RPC("SetTransform_Boss", RpcTarget.AllBuffered, new object[] { purple.GetComponent<PhotonView>().ViewID, (int)Season.Autumn });
        }
    }

    [PunRPC]
    private void SetTransform_Boss(int viewID, int season)
    {
        Transform field = bossFields[season];
        Transform boss = PhotonView.Find(viewID).transform;
        boss.SetParent(field);
        boss.localPosition = new Vector3(0, 0, 5);
    }
}
