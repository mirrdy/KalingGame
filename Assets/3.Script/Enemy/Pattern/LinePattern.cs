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
    private void Awake()
    {
        flowers = new GameObject[flowerCount];
        Vector3 pos = transform.position;
        for (int i = 0; i < flowerCount; i++)
        {
            flowers[i] = Instantiate(prefab_Flower, new Vector3(pos.x, pos.y+(i*0.5f), pos.z), Quaternion.identity);
            flowers[i].transform.SetParent(transform);
            flowers[i].SetActive(false);
        }
    }
    private void OnEnable()
    {
        createLine_Co = StartLinePattern_Co();
        StartCoroutine(createLine_Co);
    }
    private void OnDisable()
    {
        StopCoroutine(createLine_Co);
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
}
