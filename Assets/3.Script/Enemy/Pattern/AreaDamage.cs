using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : MonoBehaviour
{
    public Weather weather;
    public int damage;
    public int weatherPower;
    public float tickRate;
    private PlayerControl player;
    private float triggerStartTime;


    private ParticleSystem ps;
    public bool enter;
    public bool exit;
    public bool inside;
    public bool outside;


    private void Start()
    {
        TryGetComponent(out ps);
    }
    private void OnParticleCollision(GameObject other)
    {
        if (player == null)
        {
            if (other.TryGetComponent(out player))
            {
                StartCoroutine(StartDamage());
            }
        }
    }
    private void OnParticleTrigger()
    {
        if (enter)
        {
            List<ParticleSystem.Particle> enterList = new List<ParticleSystem.Particle>();
            int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);

            if (player != null)
            {
                StartCoroutine(StartDamage());
            }

            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);
        }

        if (exit)
        {
            List<ParticleSystem.Particle> exitList = new List<ParticleSystem.Particle>();
            int numExit = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exitList);

            

            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Exit, exitList);
        }

        if (inside)
        {
            List<ParticleSystem.Particle> insideList = new List<ParticleSystem.Particle>();
            int numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideList);

            

            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideList);
        }

        if (outside)
        {
            List<ParticleSystem.Particle> outsideList = new List<ParticleSystem.Particle>();
            int numOutside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Outside, outsideList);

            

            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Outside, outsideList);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        player = null;
    }
    private IEnumerator StartDamage()
    {
        WaitForSeconds delay = new WaitForSeconds(tickRate);

        while(player != null)
        {
            player.OnDamage(damage, weather, weatherPower);
            yield return delay;
        }
    }
    private void OnDisable()
    {
        player = null;
    }
    
    private void Update()
    {
        var trigger = ps.trigger;
        trigger.enter = enter ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;
        trigger.exit = exit ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;
        trigger.inside = inside ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;
        trigger.outside = outside ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;
    }
}
