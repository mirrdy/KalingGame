using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LineSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefab_Patt_Line;
    public Weather lineType = Weather.None;
    [SerializeField] private int lineCount = 20;

    private GameObject[] lines;
    private IEnumerator createLine_Co;

    private void OnEnable()
    {
        if (createLine_Co != null)
        {
            createLine_Co = StartLineCreate_Co();
            StartCoroutine(createLine_Co);
        }
    }
    private void Start()
    {
        lines = new GameObject[lineCount];
        Vector3 pos = transform.position;

        for (int i = 0; i < lineCount; i++)
        {
            lines[i] = Instantiate(prefab_Patt_Line[(int)lineType], new Vector3((Random.Range(-1f, 1f) * i + pos.x), pos.y, (Random.Range(-1f, 1f) * i + pos.z)), Quaternion.identity);
            lines[i].transform.SetParent(transform);
            lines[i].SetActive(false);
        }

        createLine_Co = StartLineCreate_Co();
        StartCoroutine(createLine_Co);
    }
    private void OnDisable()
    {
        if (createLine_Co != null)
        {
            StopCoroutine(createLine_Co);
        }
    }
    private IEnumerator StartLineCreate_Co()
    {
        if (lineCount != lines.Length)
        {
            Debug.Log($"{name}(LinePattern) 풀링 실패");
            yield break;
        }

        WaitForSeconds waitSec = new WaitForSeconds(1f);
        Vector3 pos = transform.position;

        for (int i = lineCount - 1; i >= 0; i--)
        {
            lines[i].transform.localPosition = new Vector3((Random.Range(-1f, 1f) * i + pos.x), pos.y, (Random.Range(-1f, 1f) * i + pos.z));
            lines[i].SetActive(true);
            yield return null;
        }
        yield return new WaitUntil(() => System.Array.TrueForAll(lines, (line) => !line.activeSelf));
        gameObject.SetActive(false);
    }
}
