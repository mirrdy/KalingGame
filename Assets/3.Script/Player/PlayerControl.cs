using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviourPunCallbacks
{
    [SerializeField] public int maxHp { get; private set; } = 100;
    public int currentHp;
    [SerializeField] private bool isDead = false;

    public delegate void WhenPlayerDie();
    public event WhenPlayerDie whenPlayerDie;

    private Vector3 spawnPos;
    private PhotonView PV;
    [SerializeField] private float flowerHitDelay = 3f;
    private float hitTime = 0;

    private void Awake()
    {
		TryGetComponent(out PV);
        currentHp = maxHp;
        spawnPos = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDamage(int damage, Season season, int seasonGaugeValue)
    {
        if(!PV.IsMine)
        {
            return;
        }
        currentHp -= damage;
        Debug.Log($"HP:{currentHp}");
        PV.RPC("AddSeasonGauge_RPC", RpcTarget.MasterClient, season, seasonGaugeValue);
        if (currentHp <= 0) //죽었을때
        {
            currentHp = 0;
            isDead = true;
            whenPlayerDie?.Invoke();
            Die();
        }
        //uiManager.HpCheck(status.maxHp, status.currentHp);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PV.IsMine)
        {
            return;
        }
        if (other.TryGetComponent(out FlowerPattern flower))
        {
            if (Time.realtimeSinceStartup - hitTime >= flowerHitDelay)
            {
                hitTime = Time.realtimeSinceStartup;
                Debug.Log($"tag:{other.tag}, name:{other.name}");
                PV.RPC("AddSeasonGauge_RPC", RpcTarget.MasterClient, flower.season, 60);
            }
        }
    }

    private void Die()
    {
        NetworkManager.instance.AddSharedLife(-100);
        // CharacterController가 transform을 update 할 때 trigger에서 변경한 position을 반영하지 않는 것 같음 - 컨트롤러를 끄고 position 변경 후 다시 킴
        if (TryGetComponent(out CharacterController control))
        {
            SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();

            control.enabled = false;
            renderer.enabled = false;

            transform.position = spawnPos;
            UIManager.instance.DisplayRespawnUI();
        }
        currentHp = maxHp;
    }

    [PunRPC]
	public void AddSeasonGauge_RPC(Season season, int value)
	{
        NetworkManager.instance.Enqueue_AddSeasonGauge(season, value);
    }
}
