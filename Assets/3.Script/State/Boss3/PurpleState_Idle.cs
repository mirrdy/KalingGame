using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleState_Idle : EntityState
{
    private BossControl boss;

    public override void EnterState(LivingEntity entity)
    {
        entity.TryGetComponent(out boss);
    }

    public override void UpdateState(LivingEntity entity)
    {
        if (NetworkManager.instance.CheckThisIsMaster())
        {
            Vector3 bossPos = boss.transform.position;
            float radius = 20f;
            int layerMask = LayerMask.GetMask("Player");

            Collider[] colliders = Physics.OverlapSphere(bossPos, radius, layerMask);

            if (colliders.Length > 0)
            {
                float minDistance = 9999;
                float distance = 0;

                Collider targetCollider = colliders[0];

                for (int i = 0; i < colliders.Length; i++)
                {
                    Collider collider = colliders[i];
                    distance = Vector3.Distance(boss.transform.position, collider.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        targetCollider = collider;
                    }
                }
                boss.SetTarget(targetCollider.transform);
            }
        }
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


            if (distance <= entity.attackRange)
            {
                boss.ChangeState(new PurpleState_Chase());
            }
            else if (distance > entity.attackRange && distance <= entity.attackRange*2)
            {
                boss.ChangeState(new PurpleState_ClawAttack());
            }
            else if (distance > entity.attackRange*2)
            {
                boss.ChangeState(new PurpleState_ClawAttack());
            }
        }
        else
        {
            
        }
    }
    public override void ExitState(LivingEntity entity)
    {

    }
}
