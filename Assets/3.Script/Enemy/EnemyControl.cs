using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{

    private void Awake()
    {
        //currentState = new MonsterIdleState();
        //ChangeState(new MonsterIdleState());
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void TakeHitRPC(float damage)
    {
        //Health -= damage;
        GetComponent<Animator>().SetTrigger("Hit");
    }
}
