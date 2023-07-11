using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControl : MonoBehaviourPunCallbacks
{
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
