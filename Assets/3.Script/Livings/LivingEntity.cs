using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    public float hp { get; protected set; }

    public float currentHp;
    public float def { get; protected set; }
    public float atk { get; protected set; }
    public float attackTime { get; protected set; }
    public float moveSpeed { get; protected set; }
    public float attackRange { get; protected set; }
    public float attackDelay {get; protected set;}
    public bool isDead { get; protected set; }

    public Animator animator;

    protected EntityState currentState;

    [SerializeField] public float timebetAttack = 0.5f;
    public float lastAttackTimebet;

    protected virtual void Start()
    {
        // 초기상태 설정할 것
        currentHp = hp;
        TryGetComponent(out animator);
        currentState = new IdleState();
        ChangeState(currentState);
    }
    protected virtual void Update()
    {
        currentState.UpdateState(this);
    }
    public void ChangeState(EntityState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public virtual void TakeDamage(int damage)
    {
        damage -= Mathf.RoundToInt(damage * (1-def));
        currentHp -= damage;
    }
}
