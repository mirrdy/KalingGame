using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Green : EnemyControl
{
    private LineSpawner[] lineSpawners;
    private readonly int lineSpawnerCount = 3;
    private IEnumerator startLineSpawn_Co;
    private PhotonView PV;
    private int[] viewIDs_LineSpawner;

    private void Awake()
    {
        lineSpawners = new LineSpawner[3];
        TryGetComponent(out PV);
        viewIDs_LineSpawner = new int[lineSpawnerCount];
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!NetworkManager.instance.CheckThisIsMaster())
        {
            return;
        }
        for (int i = 0; i < lineSpawnerCount; i++)
        {
            if (PhotonNetwork.Instantiate("LineSpawner", Vector3.zero, Quaternion.identity).TryGetComponent(out lineSpawners[i]))
            {
                viewIDs_LineSpawner[i] = lineSpawners[i].GetComponent<PhotonView>().ViewID;
            }
        }
        PV.RPC("SetTransform", RpcTarget.AllBuffered, new object[] { viewIDs_LineSpawner });
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator StartLineSpawn_Co()
    {
        WaitForSeconds spawnDelay = new WaitForSeconds(3f);
        while (true)
        {
            for (int i = 0; i < lineSpawnerCount; i++)
            {
                lineSpawners[i].gameObject.SetActive(true);

                yield return new WaitUntil(() => !lineSpawners[i].gameObject.activeSelf);
                //lineSpawners[i].enabled = true;
                //yield return new WaitUntil(() => !lineSpawners[i].enabled);
                yield return spawnDelay;
            }
        }
    }

    [PunRPC]
    private void SetTransform(int[] viewIDs)
    {
        for (int i = 0; i < viewIDs.Length; i++)
        {
            if (PhotonView.Find(viewIDs[i]).TryGetComponent(out LineSpawner lineSpawner))
            {
                lineSpawner.gameObject.SetActive(false); // 프리팹이 이미 Active false인 상태이긴 함
                lineSpawner.lineType = (Weather)i;  // Spring, Summer, Autumn
                lineSpawner.transform.SetParent(transform);
                lineSpawner.transform.localPosition = Vector3.zero;

                lineSpawners[i] = lineSpawner; // Master Client 기준에서는 중복선언
            }
        }
        startLineSpawn_Co = StartLineSpawn_Co();
        StartCoroutine(startLineSpawn_Co);
    }
}
