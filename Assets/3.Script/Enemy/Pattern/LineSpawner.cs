using Photon.Pun;
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

    private PhotonView PV;
    private int[] viewIDs_Line;
    private void Awake()
    {
        lines = new GameObject[lineCount];
        viewIDs_Line = new int[lineCount];
    }
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
        if (!NetworkManager.instance.CheckThisIsMaster())
        {
            return;
        }

        for (int i = 0; i < lineCount; i++)
        {
            lines[i] = PhotonNetwork.Instantiate(prefab_Patt_Line[(int)lineType].name, Vector3.zero, Quaternion.identity);
            viewIDs_Line[i] = lines[i].GetComponent<PhotonView>().ViewID;
        }

        if (TryGetComponent(out PV))
        {
            PV.RPC("SetTransform", RpcTarget.AllBuffered, new object[] { viewIDs_Line });
        }
        else
        {
            Debug.Log($"PhotonView Component can not find in {gameObject.name}");
        }
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

        Vector3 pos = Vector3.zero;

        for (int i = lineCount - 1; i >= 0; i--)
        {
            lines[i].transform.localPosition = new Vector3((Random.Range(-1f, 1f) * i + pos.x), pos.y, (Random.Range(-1f, 1f) * i + pos.z));
            lines[i].SetActive(true);
            yield return null;
        }
        yield return new WaitUntil(() => System.Array.TrueForAll(lines, (line) => !line.activeSelf));
        gameObject.SetActive(false);
    }

    [PunRPC]
    private void SetTransform(int[] viewIDs)
    {
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < viewIDs.Length; i++)
        {
            if (PhotonView.Find(viewIDs[i]).TryGetComponent(out LinePattern line))
            {
                lines[i] = line.gameObject;
                line.transform.SetParent(transform);
                line.transform.localPosition = new Vector3((Random.Range(-1f, 1f) * i + pos.x), pos.y, (Random.Range(-1f, 1f) * i + pos.z));
                line.gameObject.SetActive(false);
            }
        }
        createLine_Co = StartLineCreate_Co();
        StartCoroutine(createLine_Co);
    }
}
