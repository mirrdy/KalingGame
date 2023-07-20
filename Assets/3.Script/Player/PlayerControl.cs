using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviourPunCallbacks
{
    [SerializeField] public int maxHp { get; private set; } = 100;
    public int currentHp;
    [SerializeField] public int maxMp { get; private set; } = 100;
    public int currentMp;

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
        SetCurrentMp(maxMp);
        spawnPos = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead && PV.IsMine)
        {
            CheckUsePotion();
        }
    }

    private void CheckUsePotion()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (UIManager.instance.UsePotion(PotionType.HP))
            {
                SetCurrentHp(currentHp + 50);
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (UIManager.instance.UsePotion(PotionType.MP))
            {
                SetCurrentMp(currentMp + 50);
            }
        }
    }
    public void OnDamage(int damage, Season season, int seasonGaugeValue)
    {
        if (!PV.IsMine)
        {
            return;
        }

        SetCurrentHp(currentHp - damage);
        Debug.Log($"HP:{currentHp}");
        PV.RPC("AddSeasonGauge_RPC", RpcTarget.MasterClient, season, seasonGaugeValue);

        if (currentHp <= 0) //�׾�����
        {
            currentHp = 0;
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
                OnDamage(10, flower.season, 100);
            }
        }
    }
    public void SetCurrentHp(int hp)
    {
        if (hp < 0)
        {
            hp = 0;
        }
        else if (hp > maxHp)
        {
            hp = maxHp;
        }
        currentHp = hp;
        UIManager.instance.HpCheck(maxHp, currentHp);
    }
    public void SetMaxHp(int hp)
    {
        if (hp < 0)
        {
            hp = 0;
        }
        maxHp = hp;
        UIManager.instance.HpCheck(maxHp, currentHp);
    }
    public void SetCurrentMp(int mp)
    {
        if (mp < 0)
        {
            mp = 0;
        }
        else if (mp > maxMp)
        {
            mp = maxMp;
        }
        currentMp = mp;
        UIManager.instance.MpCheck(maxMp, currentMp);
    }
    public void SetMaxMp(int mp)
    {
        if(mp<0)
        {
            mp = 0;
            maxMp = mp;
            UIManager.instance.MpCheck(maxMp, currentMp);
        }
    }
    private void Die()
    {
        isDead = true;
        NetworkManager.instance.AddSharedLife(-100);
        // CharacterController�� transform�� update �� �� trigger���� ������ position�� �ݿ����� �ʴ� �� ���� - ��Ʈ�ѷ��� ���� position ���� �� �ٽ� Ŵ
        if (TryGetComponent(out CharacterController control))
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach(Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }

            control.enabled = false;
            UIManager.instance.DisplayRespawnUI();
        }
    }
    public void Respawn()
    {
        // CharacterController�� transform�� update �� �� trigger���� ������ position�� �ݿ����� �ʴ� �� ���� - ��Ʈ�ѷ��� ���� position ���� �� �ٽ� Ŵ
        if (TryGetComponent(out CharacterController control))
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }

            control.enabled = true;
        }
        SetCurrentHp(maxHp);
        SetCurrentMp(maxMp);
        transform.position = spawnPos;
        isDead = false;
    }

    [PunRPC]
	public void AddSeasonGauge_RPC(Season season, int value)
	{
        NetworkManager.instance.Enqueue_AddSeasonGauge(season, value);
    }
}
