using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathMaker : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    private ParticleSystem breath;
    public Transform target;
    IEnumerator breathe_Co;
    private float startMakeTime;

    private void Awake()
    {
        breath = Instantiate(ps, transform.position + Vector3.back, Quaternion.identity);
        breath.gameObject.SetActive(false);
        breathe_Co = Breathe_Co();
    }
    private void OnEnable()
    {
        StartCoroutine(breathe_Co);
    }
    private void OnDisable()
    {
        StopCoroutine(breathe_Co);
    }
    private IEnumerator Breathe_Co()
    {
        startMakeTime = Time.time;
        while(true)
        {
            if(Time.time > startMakeTime+1)
            {
                yield break;
            }
            yield return null;
        }
        
    }
}
