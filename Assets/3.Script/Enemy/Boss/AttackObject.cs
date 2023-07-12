using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObject : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int weatherPower = 200;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EnemyControl boss = GetComponentInParent<EnemyControl>();
            if (!boss.isDead && Time.time >= boss.lastAttackTimebet + boss.timebetAttack)
            {
                //boss.lastAttackTimebet = Time.time;
                other.TryGetComponent(out PlayerControl player);
                damage = Mathf.RoundToInt(GetComponentInParent<EnemyControl>().atk);
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;
                player.OnDamage(damage, boss.weather, weatherPower);
                Debug.Log($"¸Â¾ÒÀ½");
            }
        }
    }
}
