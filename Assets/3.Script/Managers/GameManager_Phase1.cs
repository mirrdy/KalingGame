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
    private LineSpawner[] lineSpawners;
    private readonly int lineSpawnerCount = 3;
    private IEnumerator startLineSpawn_Co;

    private void Awake()
    {
        lineSpawners = new LineSpawner[3];
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
            for(int i=0; i<lineSpawnerCount; i++)
            {
                if(PhotonNetwork.Instantiate("LineSpawner", Vector3.zero, Quaternion.identity).TryGetComponent(out lineSpawners[i]))
                {
                    lineSpawners[i].gameObject.SetActive(false); // �������� �̹� Active false�� �����̱� ��
                    lineSpawners[i].lineType = (Weather)i;  // Spring, Summer, Autumn
                }
            }
            startLineSpawn_Co = StartLineSpawn_Co();
            StartCoroutine(startLineSpawn_Co);
        }
    }

    public void UpdateSharedLife()
    {
        text_SharedLife.text = NetworkManager.instance.GetSharedLife().ToString();
    }
    private IEnumerator StartLineSpawn_Co()
    {
        WaitForSeconds spawnDelay = new WaitForSeconds(3f);
        while(true)
        {
            for(int i=0; i<lineSpawnerCount; i++)
            {
                lineSpawners[i].gameObject.SetActive(true);

                yield return new WaitUntil(() => !lineSpawners[i].gameObject.activeSelf);
                //lineSpawners[i].enabled = true;
                //yield return new WaitUntil(() => !lineSpawners[i].enabled);
                yield return spawnDelay;
            }
        }
    }
}
