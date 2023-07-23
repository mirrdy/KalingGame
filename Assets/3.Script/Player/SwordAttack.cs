using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public int damage = 5;
    public bool canAttack = true;
    private Transform[] targets; // 타겟 여러명일 때 개체별 타격 쿨타임 적용해야함
    private float attackStartTime;
    private Animator bossAnimator;
    private bool isReadyAttack;

    private const int FRAME_SECOND_ATTACK_START = 46;
    private const int FRAME_THIRD_ATTACK_START = 75;

    private void Awake()
    {
        bossAnimator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        AnimatorStateInfo aniState = bossAnimator.GetCurrentAnimatorStateInfo(0);
        if (!aniState.IsName("TripleSlash"))
        {
            damage = 5;
            return;
        }

        AnimatorClipInfo[] clipInfo = bossAnimator.GetCurrentAnimatorClipInfo(0);
        AnimationClip clip = clipInfo[0].clip;
        float sampleRate = clip.frameRate;

        if (aniState.normalizedTime >= (FRAME_THIRD_ATTACK_START / sampleRate) / clip.length)
        {
            damage = 45;
        }
        else if (aniState.normalizedTime >= (FRAME_SECOND_ATTACK_START / sampleRate) / clip.length)
        {
            damage = 15;
        }
        else
        {
            damage = 5;
        }
        Debug.Log($"damage:{damage}");
    }
    private void OnTriggerEnter(Collider other)
    {
        BossControl boss = other.GetComponentInParent<BossControl>();
        if (boss != null)
        {
            if (Time.time - attackStartTime > 0.5f)
            {
                boss.TakeDamage(damage);
                attackStartTime = Time.time;
            }
        }
    }
}
