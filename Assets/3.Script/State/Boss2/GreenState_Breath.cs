using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenState_Breath : EntityState
{
    private Transform breathPos;
    private Transform target;
    private float time = 0;
    private EnemyControl boss;
    public override void EnterState(LivingEntity entity)
    {
        SetBreathPos(entity);
        InitTarget(entity);
        entity.animator.SetBool("isScream", true);
    }
    private void SetBreathPos(LivingEntity entity)
    {
        breathPos = entity.GetComponentInChildren<BreathMaker>().transform;
    }
    private void InitTarget(LivingEntity entity)
    {
        if(entity.TryGetComponent(out boss))
        {
            target = boss.target;
        }
    }
    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isScream", false);
    }

    public override void UpdateState(LivingEntity entity)
    {
        // ���Ͱ� �÷��̾� ���� �ٶ󺸵��� ȸ�� ����
        Vector3 playerDirection = boss.target.position - entity.transform.position;

        playerDirection.y = 0; // Y �� ������ �����Ͽ� ��� ���� ���⸸ ����մϴ�.
        Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
        boss.transform.rotation = targetRotation;
    }
}
