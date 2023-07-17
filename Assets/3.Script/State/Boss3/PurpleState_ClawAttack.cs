using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleState_ClawAttack : EntityState
{
    private Vector3 targetPos;
    private Vector3 startPos;
    private Vector3 direction;
    private const int FRAME_DASH_START = 35;
    private const int FRAME_DASH_END = 61;

    public override void EnterState(LivingEntity entity)
    {
        SetTargetPos(entity);
        entity.animator.SetBool("isClawAttack", true);
        //AudioManager.instance.PlaySFX("ClawAttack");
    }
    private void SetTargetPos(LivingEntity entity)
    {
        if (entity.TryGetComponent(out BossControl boss))
        {
            startPos = boss.transform.position;
            targetPos = boss.target.position;
            direction = targetPos - startPos;
        }
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isClawAttack", false);
        entity.animator.Play(entity.animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);
    }

    public override void UpdateState(LivingEntity entity)
    {
        AnimatorStateInfo aniState = entity.animator.GetCurrentAnimatorStateInfo(0);
        if (!aniState.IsName("Claw Attack"))
        {
            return;
        }

        AnimatorClipInfo[] clipInfo = entity.animator.GetCurrentAnimatorClipInfo(0);
        AnimationClip clip = clipInfo[0].clip;
        float sampleRate = clip.frameRate;

        float dashTime = (FRAME_DASH_END - FRAME_DASH_START) / sampleRate;

        if (aniState.normalizedTime >= (FRAME_DASH_START / sampleRate) / clip.length &&
            aniState.normalizedTime <= (FRAME_DASH_END / sampleRate) / clip.length)
        {
            entity.transform.position += (direction / (dashTime / Time.deltaTime));
            
        }
    }
}
