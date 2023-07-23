using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossControl : EnemyControl
{
    [SerializeField] private EnemyData data_Stat;
    private LineSpawner[] lineSpawners;
    private readonly int lineSpawnerCount = 3;
    private IEnumerator startLineSpawn_Co;
    private PhotonView PV;
    private int[] viewIDs_LineSpawner;
    public AttackObject[] atkObjects;
    public Slider healthSlider; // ���� ü�¹� Slider UI ���

    private void Awake()
    {
        InitStatData();
        SetTrigger();
        lineSpawners = new LineSpawner[3];
        TryGetComponent(out PV);
        healthSlider = GetComponentInChildren<Slider>();
        viewIDs_LineSpawner = new int[lineSpawnerCount];

        if (season == Season.Spring)
        {
            currentState = new YellowState_Idle();
        }
        else if (season == Season.Summer)
        {
            currentState = new GreenState_Idle();
        }
        else if (season == Season.Autumn)
        {
            currentState = new PurpleState_Idle();
        }
        SetIdleState();
    }
    private void SetTrigger()
    {
        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach(Collider col in cols)
        {
            // �ǰ� ���� ON
            col.isTrigger = true;
            col.tag = "Enemy";
        }

        // ���� ������ ���� �Ҷ��� ų ����
        atkObjects = GetComponentsInChildren<AttackObject>();
        foreach(AttackObject atkObj in atkObjects)
        {
            atkObj.triggerEnabled = false;
        }
    }
    private void InitStatData()
    {
        maxHp = data_Stat.maxHp;
        atk = data_Stat.atk;
        def = data_Stat.def;
        attackTime = data_Stat.attackTime;
        moveSpeed = data_Stat.moveSpeed;
        attackRange = data_Stat.attackRange;
        attackDelay = data_Stat.attackDelay;
        season = data_Stat.weather;
        
        canAttack = true;
    }
    public void InitAttackPoint()
    {
        foreach(AttackObject atkObj in atkObjects)
        {
            atkObj.triggerEnabled = false;
        }
    }
    public void EndAttack()
    {
        StartCoroutine(AttackTime_co());
    }
    private IEnumerator AttackTime_co()
    {
        canAttack = false;
        SetIdleState();
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }
    private void SetIdleState()
    {
        if (season == Season.Spring)
        {
            ChangeState(new YellowState_Idle());
        }
        else if (season == Season.Summer)
        {
            ChangeState(new GreenState_Idle());
        }
        else if (season == Season.Autumn)
        {
            ChangeState(new PurpleState_Idle());
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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
    protected override void Update()
    {
        if(isDead)
        {
            return;
        }
        base.Update();
    }
    public override void TakeDamage(int damage)
    {
        if(isDead)
        {
            return;
        }
        base.TakeDamage(damage);
        PV.RPC("TakeDamage_RPC", RpcTarget.AllBuffered, new object[] { damage });
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
    private void Die()
    {
        Debug.Log($"{season} Boss dies");
        animator.SetTrigger("Die");
        isDead = true;
        StartCoroutine(Destroy_Co());
    }

    IEnumerator Destroy_Co()
    {
        // until AniState�� ����ü�� �� ������ �����ؼ� ���� ������ WaitUntil �ۿ��� ĳ���ؼ� ���� ���� State�� ����ȭ�� �ȵ�
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        Destroy(gameObject);
    }

    [PunRPC]
    private void SetTransform(int[] viewIDs)
    {
        for (int i = 0; i < viewIDs.Length; i++)
        {
            if (PhotonView.Find(viewIDs[i]).TryGetComponent(out LineSpawner lineSpawner))
            {
                lineSpawner.gameObject.SetActive(false); // �������� �̹� Active false�� �����̱� ��
                lineSpawner.lineType = (Season)i;  // Spring, Summer, Autumn
                lineSpawner.transform.SetParent(transform.parent);
                lineSpawner.transform.localPosition = Vector3.zero;

                lineSpawners[i] = lineSpawner; // Master Client ���ؿ����� �ߺ�����
            }
        }
        startLineSpawn_Co = StartLineSpawn_Co();
        StartCoroutine(startLineSpawn_Co);
    }
    [PunRPC]
    private void TakeDamage_RPC(int damage)
    {
        damage = Mathf.RoundToInt(damage * (1 - def));
        Debug.Log($"{season} Boss takes {damage}damages");
        currentHp -= damage;
        UpdateHealthBar();
        if (currentHp <= 0)
        {
            Die();
        }
    }
    void UpdateHealthBar()
    {
        healthSlider.value = currentHp / maxHp;
    }
}
