using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab_Patt_Line;

    [SerializeField] private int lineCount = 20;

    private GameObject[] lines;
    private IEnumerator createLine_Co;

    private void Awake()
    {
        lines = new GameObject[lineCount];
        Vector3 pos = transform.position;
        for (int i = 0; i < lineCount; i++)
        {
            lines[i] = Instantiate(prefab_Patt_Line, new Vector3((Random.Range(0, 0.2f)*i + pos.x), pos.y, (Random.Range(0, 0.2f) * i + pos.z)), Quaternion.identity);
            lines[i].transform.SetParent(transform);
            lines[i].SetActive(false);
        }
        createLine_Co = StartLineCreate_Co();
    }
    private void OnEnable()
    {
        StartCoroutine(createLine_Co);
    }
    private void OnDisable()
    {

    }
    private IEnumerator StartLineCreate_Co()
    {
        if (lineCount != lines.Length)
        {
            Debug.Log($"{name}(LinePattern) 풀링 실패");
            yield break;
        }

        WaitForSeconds waitSec = new WaitForSeconds(4f);
        for (int i = lineCount - 1; i >= 0; i--)
        {
            lines[i].SetActive(true);
            //yield return waitSec;
            yield return null;
        }
    }
}
