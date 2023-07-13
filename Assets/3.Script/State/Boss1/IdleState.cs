using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : EntityState
{
    private Boss_Green boss;

    public override void EnterState(LivingEntity entity)
    {
        entity.TryGetComponent(out boss);
    }

    public override void UpdateState(LivingEntity entity)
    {
        if (boss.target != null)
        {
            Vector3 targetPosition = boss.target.transform.position;
            targetPosition.y = boss.transform.position.y;

            Vector3 direction = targetPosition - boss.transform.position;
            direction.Normalize();

            float distance = Vector3.Distance(boss.transform.position, targetPosition);

            Vector3 playerDirection = boss.target.position - boss.transform.position;
            playerDirection.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(playerDirection);

            boss.transform.rotation = targetRotation;

            if (distance <= entity.attackRange && boss.canAttack)
            {
                boss.ChangeState(new ChaseState());
            }
            else if (distance > entity.attackRange)
            {
                boss.ChangeState(new JumpState());
            }
        }
        else
        {
            Vector3 bossPos = boss.transform.position;
            float radius = 20f; 
            int layerMask = LayerMask.GetMask("Player");

            Collider[] colliders = Physics.OverlapSphere(bossPos, radius, layerMask);

            foreach (Collider collider in colliders)
            {
                boss.target = collider.transform;
                break;
            }
        }
    }
    public override void ExitState(LivingEntity entity)
    {

    }
    
}
