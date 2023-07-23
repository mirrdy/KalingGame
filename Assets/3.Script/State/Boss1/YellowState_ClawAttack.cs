using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowState_ClawAttack : EntityState
{
    public override void EnterState(LivingEntity entity)
    {
        entity.animator.SetBool("isClawAttack", true);
        //AudioManager.instance.PlaySFX("ClawAttack");
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isClawAttack", false);
    }

    public override void UpdateState(LivingEntity entity)
    {
        
    }

}
