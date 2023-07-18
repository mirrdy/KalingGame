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

    public delegate void WhenPlayerDamaged();
    public event WhenPlayerDamaged whenPlayerDamaged;
    public delegate void WhenPlayerDie();
    public event WhenPlayerDie whenPlayerDie;

    public Vector3 spawnPos { get; private set; }
    private PhotonView PV;
    [SerializeField] private float flowerHitDelay = 3f;
    private float hitTime = 0;

    private void Awake()
    {
		TryGetComponent(out PV);
        SetCurrentHp(maxHp);
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

        SetCurrentHp(currentHp - damage);
        Debug.Log($"HP:{currentHp}");
        PV.RPC("AddSeasonGauge_RPC", RpcTarget.MasterClient, season, seasonGaugeValue);
        
        if (currentHp <= 0) //�׾�����
        {
            currentHp = 0;
            isDead = true;
            whenPlayerDie?.Invoke();
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PV.IsMine)
        {
            return;
        }
        // �ɽ� ���� �ߺ�Ÿ�� ���⼭ ó����
        if (other.TryGetComponent(out FlowerPattern flower))
        {
            if (Time.realtimeSinceStartup - hitTime >= flowerHitDelay)
            {
                hitTime = Time.realtimeSinceStartup;
                Debug.Log($"tag:{other.tag}, name:{other.name}");
                PV.RPC("AddSeasonGauge_RPC", RpcTarget.MasterClient, flower.season, 60);
                SetCurrentHp(currentHp - 10);
            }
        }
    }
    public void SetCurrentHp(int hp)
    {
        if(hp < 0)
        {
            hp = 0;
        }
        else if(hp > maxHp)
        {
            hp = maxHp;
        }
        currentHp = hp;
        UIManager.instance.HpCheck(maxHp, currentHp);
    }
    public void SetMaxHp(int hp)
    {
        if(hp < 0)
        {
            hp = 0;
        }
        maxHp = hp;
        UIManager.instance.HpCheck(maxHp, currentHp);
    }
    private void Die()
    {
        NetworkManager.instance.AddSharedLife(-100);
        // CharacterController�� transform�� update �� �� trigger���� ������ position�� �ݿ����� �ʴ� �� ���� - ��Ʈ�ѷ��� ���� position ���� �� �ٽ� Ŵ
        if (TryGetComponent(out CharacterController control))
        {
            SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();

            control.enabled = false;
            renderer.enabled = false;

            UIManager.instance.DisplayRespawnUI();
        }
    }

    [PunRPC]
	public void AddSeasonGauge_RPC(Season season, int value)
	{
        NetworkManager.instance.Enqueue_AddSeasonGauge(season, value);
    }
}
