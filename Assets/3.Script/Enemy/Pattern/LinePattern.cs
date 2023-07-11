using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePattern : MonoBehaviour
{
    [SerializeField] private GameObject prefab_Flower;
    [SerializeField] private int flowerCount = 20;

    private GameObject[] flowers;
    private IEnumerator createLine_Co;

    private PhotonView PV;
    private void Awake()
    {
        TryGetComponent(out PV);
        flowers = new GameObject[flowerCount];
        for (int i = 0; i < flowerCount; i++)
        {
            Vector3 pos = transform.position;
            flowers[i] = Instantiate(prefab_Flower, new Vector3(pos.x, pos.y + (i * 0.5f), pos.z), Quaternion.identity);
            flowers[i].transform.SetParent(transform);
            flowers[i].SetActive(false);
        }
        //if (NetworkManager.instance.CheckThisIsMaster())
        //{
        //    PV.RPC("SetTransform", RpcTarget.AllBuffered);
        //}
    }
    private void OnEnable()
    {
        StartCreateLine();
        //if (NetworkManager.instance.CheckThisIsMaster())
        //{       
        //    PV.RPC("StartCreateLine", RpcTarget.AllBuffered);
        //}
    }
    private void OnDisable()
    {
        StopCreateLine();
        //if (NetworkManager.instance.CheckThisIsMaster())
        //{
        //    PV.RPC("StopCreateLine", RpcTarget.AllBuffered);
        //}
    }

    IEnumerator StartLinePattern_Co()
    {
        if (flowerCount != flowers.Length)
        {
            Debug.Log($"{name}(LinePattern) 풀링 실패");
            yield break;
        }

        WaitForSeconds waitSec = new WaitForSeconds(0.02f);
        for (int i = flowerCount-1; i >= 0; i--)
        {
            flowers[i].SetActive(true);
            yield return waitSec;
        }
        //yield return new WaitUntil(() => System.Array.TrueForAll(flowers, (flower) => !flower.activeSelf));
        int lastOffIndex = 0;
        yield return new WaitUntil(() => flowers[lastOffIndex].activeSelf == false);
        gameObject.SetActive(false);
    }

    [PunRPC]
    private void SetTransform()
    {   
        for (int i = 0; i < flowerCount; i++)
        {
            flowers[i].transform.SetParent(transform);
            flowers[i].SetActive(false);
        }
    }
    [PunRPC]
    private void StartCreateLine()
    {
        createLine_Co = StartLinePattern_Co();
        StartCoroutine(createLine_Co);
    }
    [PunRPC]
    private void StopCreateLine()
    {
        StopCoroutine(createLine_Co);
    }
}
