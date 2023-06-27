using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityState
{
    public abstract void UpdateState(LivingEntity entity);
    public abstract void EnterState(LivingEntity entity);
    public abstract void ExitState(LivingEntity entity);

}
