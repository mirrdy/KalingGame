using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : LivingEntity
{
    public Transform target;
    public bool canAttack;
    public bool isAttackTriggerOn;
    public Weather weather;

    private void Awake()
    {
        //currentState = new MonsterIdleState();
        //ChangeState(new MonsterIdleState());
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    [PunRPC]
    public virtual void Move()
    {

    }
    [PunRPC]
    public virtual void TakeHitRPC(float damage)
    {
        //Health -= damage;
        GetComponent<Animator>().SetTrigger("Hit");
    }
}
