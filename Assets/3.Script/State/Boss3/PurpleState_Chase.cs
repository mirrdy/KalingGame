using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleState_Chase : EntityState
{
    BossControl boss;
    public override void EnterState(LivingEntity entity)
    {
        entity.animator.SetBool("isWalk", true);
        entity.TryGetComponent(out boss);
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isWalk", false);
    }

    public override void UpdateState(LivingEntity entity)
    {
        if (boss.target != null)
        {
            Vector3 targetPosition = boss.target.transform.position;
            targetPosition.y = entity.transform.position.y;
            Vector3 direction = targetPosition - entity.transform.position;
            direction.Normalize();
            float distance = Vector3.Distance(boss.transform.position, targetPosition);
            boss.transform.position += (boss.moveSpeed * Time.deltaTime * direction);

            // 몬스터가 플레이어 쪽을 바라보도록 회전 설정
            Vector3 playerDirection = boss.target.position - entity.transform.position;

            playerDirection.y = 0; // Y 축 방향을 무시하여 평면 상의 방향만 고려합니다.
            Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
            boss.transform.rotation = targetRotation;

            if (boss.canAttack)
            {
                if (distance <= boss.attackRange)
                {
                    boss.ChangeState(new PurpleState_BasicAttack());
                }
                else if (distance <= boss.attackRange * 3 && distance > boss.attackRange)
                {
                    boss.ChangeState(new PurpleState_ClawAttack());
                }
                else
                {
                    boss.ChangeState(new PurpleState_ClawAttack());
                }
            }
            //else
            //{
            //    if (distance <= boss.attackRange)
            //    {
            //        boss.ChangeState(new PurpleState_Idle());
            //        Debug.Log("purple chase to idle");
            //    }
            //}
        }
        if (boss.target == null)
        {
            return;
        }
    }
}
