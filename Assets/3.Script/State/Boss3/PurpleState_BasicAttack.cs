using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleState_BasicAttack : EntityState
{
    public override void EnterState(LivingEntity entity)
    {
        entity.animator.SetBool("isBasicAttack", true);
        //AudioManager.instance.PlaySFX("ClawAttack");
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isBasicAttack", false);
    }

    public override void UpdateState(LivingEntity entity)
    {
        
    }
}
