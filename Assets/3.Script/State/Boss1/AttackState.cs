using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EntityState
{
    private IEnumerator attack_co;
    private Boss_Green boss;

    public override void EnterState(LivingEntity entity)
    {
        if (boss == null)
        {
            entity.TryGetComponent(out boss);
        }
        boss.animator.SetBool("isAttack", true);

        //AudioManager.instance.PlaySFX(entity.name + "Attack");

        //waitForSeconds = new WaitForSeconds(2f);
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isAttack", false);
        //entity.entityController.stepOffset = 1f;
    }

    public override void UpdateState(LivingEntity entity)
    {
        if (boss.target == null)
        {
            //entity.ChangeState(new MonsterReturnState());
            return;
        }
        // 몬스터가 플레이어 쪽을 바라보도록 회전 설정
        Vector3 playerDirection = boss.target.position - entity.transform.position;
        playerDirection.y = 0; // Y 축 방향을 무시하여 평면 상의 방향만 고려합니다.
        Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
        boss.transform.rotation = targetRotation;

        Vector3 direction = boss.target.position - boss.transform.position;
        direction.Normalize();
        float distance = Vector3.Distance(boss.transform.position, boss.target.position);

        //if (distance > boss.attackRange)// monster attackRange를써야함
        //{
        //    //entity.ChangeState(new MonsterChaseState());
        //}
    }
}
