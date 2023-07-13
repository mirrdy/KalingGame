using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathState : EntityState
{
    private Transform breathPos;
    public override void EnterState(LivingEntity entity)
    {
        SetBreathPos(entity);
        entity.animator.SetBool("Scream", true);
    }
    private void SetBreathPos(LivingEntity entity)
    {
        breathPos = entity.GetComponentInChildren<BreathMaker>().transform;
    }
    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("Scream", false);
    }

    public override void UpdateState(LivingEntity entity)
    {
        
    }
}
