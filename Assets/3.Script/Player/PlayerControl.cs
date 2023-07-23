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
    private Animator animator;
    [SerializeField] private float flowerHitDelay = 3f;
    private bool canAction;
    private float actionCool = 0;
    private float hitTime = 0;

    #region animation Hash
    private int animID_Speed;
    private int animID_Ground;
    private int animID_Jump;
    private int animID_Falling;
    private int animID_BasicSlash;
    private int animID_TripleSlash;
    private int animID_Die;
    private int animID_AttackSpeed;
    #endregion

    private void Awake()
    {
        TryGetComponent(out PV);
        TryGetComponent(out animator);
        SetCurrentHp(maxHp);
        SetCurrentMp(maxMp);
        AssignAnimationID();
        spawnPos = transform.position;
    }
    private void AssignAnimationID()
    {
        animID_Speed = Animator.StringToHash("Speed");
        animID_Ground = Animator.StringToHash("Ground");
        animID_Jump = Animator.StringToHash("Jump");
        animID_Falling = Animator.StringToHash("Falling");
        animID_BasicSlash = Animator.StringToHash("BasicSlash");
        animID_TripleSlash = Animator.StringToHash("TripleSlash");
        animID_Die = Animator.StringToHash("Die");

        animID_AttackSpeed = Animator.StringToHash("AttackSpeed");
        
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
            Attack();
            CheckUsePotion();
        }
        if(GameManager_Phase1.instance.isGameOver)
        {
            gameObject.SetActive(false);
        }
    }
    private void Attack()
    {
        actionCool += Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            AnimatorStateInfo aniState = animator.GetCurrentAnimatorStateInfo(0);
            if (aniState.IsName("BasicSlash"))
            {
                if(aniState.normalizedTime >= 1)
                {
                    animator.SetTrigger(animID_BasicSlash);
                }
            }
            else
            {
                animator.SetTrigger(animID_BasicSlash);
            }
            //float attackSpeed = 0.5f;
            //if(actionCool > 1f * attackSpeed)
            //{
            //    //AudioManager.instance.PlaySFX("PlayerSwordAttack");
            //    //animator.SetFloat(animID_AttackSpeed, 0.533f / attackSpeed);
            //    actionCool = 0;
            //}
        }
        if (Input.GetKey(KeyCode.Q))
        {
            float attackSpeed = 3f;
            if (actionCool > 1f * attackSpeed)
            {
                if (currentMp >= 50)
                {
                    SetCurrentMp(currentMp - 50);
                    //AudioManager.instance.PlaySFX("PlayerSwordAttack");
                    //animator.SetFloat(animID_AttackSpeed, 0.533f / attackSpeed);
                    animator.SetTrigger(animID_TripleSlash);

                    actionCool = 0;
                }
            }
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

        if (currentHp <= 0) //죽었을때
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
        // 꽃실 패턴 중복타격 여기서 처리함
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
        // CharacterController가 transform을 update 할 때 trigger에서 변경한 position을 반영하지 않는 것 같음 - 컨트롤러를 끄고 position 변경 후 다시 킴
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
        // CharacterController가 transform을 update 할 때 trigger에서 변경한 position을 반영하지 않는 것 같음 - 컨트롤러를 끄고 position 변경 후 다시 킴
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
