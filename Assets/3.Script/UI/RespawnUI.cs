using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RespawnUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text_RespawnTime;
    private PlayerControl myPlayer;
    private void Awake()
    {
        PlayerControl[] players = FindObjectsOfType<PlayerControl>();
        foreach(PlayerControl player in players)
        {
            if(player.TryGetComponent(out PhotonView PV))
            {
                if(PV.IsMine)
                {
                    myPlayer = player;
                    break;
                }
            }
        }
    }
    private void OnEnable()
    {
        StartCoroutine(CountRespawnTime_Co());
    }
    private void OnDisable()
    {
        Respawn();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator CountRespawnTime_Co()
    {
        WaitForSeconds second = new WaitForSeconds(1);
        for(int i = 10; i>0; i--)
        {
            text_RespawnTime.text = $"{i}";
            yield return second;
        }
        gameObject.SetActive(false);
    }
    public void OnButton_Respawn()
    {
        gameObject.SetActive(false);
    }

    private void Respawn()
    {
        // CharacterController가 transform을 update 할 때 trigger에서 변경한 position을 반영하지 않는 것 같음 - 컨트롤러를 끄고 position 변경 후 다시 킴
        if (myPlayer.TryGetComponent(out CharacterController control))
        {
            SkinnedMeshRenderer renderer = myPlayer.GetComponentInChildren<SkinnedMeshRenderer>();

            renderer.enabled = true;
            control.enabled = true;
        }
        myPlayer.SetCurrentHp(myPlayer.maxHp);
        myPlayer.transform.position = myPlayer.spawnPos;
    }
}
