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
        // 몬스터가 플레이어 쪽을 바라보도록 회전 설정
        Vector3 playerDirection = boss.target.position - entity.transform.position;

        playerDirection.y = 0; // Y 축 방향을 무시하여 평면 상의 방향만 고려합니다.
        Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
        boss.transform.rotation = targetRotation;
    }
}
