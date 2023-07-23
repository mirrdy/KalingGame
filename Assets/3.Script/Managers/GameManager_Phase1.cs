using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager_Phase1 : MonoBehaviour
{
    public static GameManager_Phase1 instance;
    [SerializeField] private Transform[] bossFields;
    private PhotonView PV;
    [SerializeField] private RectTransform panel_GameOver;
    public bool isGameOver;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
            Debug.Log($"플레이어 생성 후 네트워크에 PhotonView 할당 실패");
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
    private void Update()
    {
        if(NetworkManager.instance.GetSharedLife() <= 0)
        {
            StartCoroutine(GameOver_Co());
            
        }
    }
    IEnumerator GameOver_Co()
    {
        isGameOver = true;
        panel_GameOver.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("GameOver");
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
