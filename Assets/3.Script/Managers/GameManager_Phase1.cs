using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;


public class GameManager_Phase1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        PhotonNetwork.Instantiate("NightMare_Green", new Vector3(0, 0, 5), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
