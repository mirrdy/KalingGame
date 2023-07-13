using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : EntityState
{
    private Vector3 targetPos;
    private Vector3 startPos;
    private Vector3 direction;
    private const int FRAME_JUMP_START = 20;
    private const int FRAME_JUMP_END = 35;
    private float jumpStartTime;
    private bool isFinishJump;
    public override void EnterState(LivingEntity entity)
    {
        SetTargetPos(entity);
        entity.animator.SetBool("isJump", true);
        //AudioManager.instance.PlaySFX("ClawAttack");
    }
    private void SetTargetPos(LivingEntity entity)
    {
        if (entity.TryGetComponent(out Boss_Green boss))
        {
            startPos = boss.transform.position;
            targetPos = boss.target.position;
            direction = targetPos - startPos;
        }
    }

    public override void ExitState(LivingEntity entity)
    {
        entity.animator.SetBool("isJump", false);
        entity.animator.Play(entity.animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);
    }

    public override void UpdateState(LivingEntity entity)
    {
        AnimatorStateInfo aniState = entity.animator.GetCurrentAnimatorStateInfo(0);
        if (!aniState.IsName("Jump"))
        {
            return;
        }

        AnimatorClipInfo[] clipInfo = entity.animator.GetCurrentAnimatorClipInfo(0);
        AnimationClip clip = clipInfo[0].clip;
        float sampleRate = clip.frameRate;

        float jumpMoveTime = (FRAME_JUMP_END - FRAME_JUMP_START) / sampleRate;

        if (aniState.normalizedTime >= (FRAME_JUMP_START/ sampleRate) / clip.length &&
            aniState.normalizedTime <= (FRAME_JUMP_END / sampleRate) / clip.length)
        {
            entity.transform.position += (direction / (jumpMoveTime / Time.deltaTime));
        }
    }
}
