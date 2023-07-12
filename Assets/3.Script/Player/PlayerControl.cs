using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControl : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] public int currentHp { get; private set; }
    [SerializeField] private bool isDead = false;

    public delegate void WhenPlayerDie();
    public event WhenPlayerDie whenPlayerDie;

    private PhotonView PV;
    [SerializeField] private float flowerHitDelay = 3f;
    private float hitTime = 0;

    private void Awake()
    {
		TryGetComponent(out PV);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDamage(int damage, Weather weather, int weatherGaugeValue)
    {
        if(!PV.IsMine)
        {
            return;
        }
        currentHp -= damage;
        PV.RPC("AddWeatherGauge_RPC", RpcTarget.MasterClient, weather, weatherGaugeValue);
        if (currentHp <= 0) //Á×¾úÀ»¶§
        {
            currentHp = 0;
            isDead = true;
            whenPlayerDie?.Invoke();
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
                PV.RPC("AddWeatherGauge_RPC", RpcTarget.MasterClient, flower.weather, 60);
            }
        }
    }

    [PunRPC]
	public void AddWeatherGauge_RPC(Weather weather, int value)
	{
        NetworkManager.instance.Enqueue_AddWeatherGauge(weather, value);
    }
}
