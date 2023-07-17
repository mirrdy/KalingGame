using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPattern : MonoBehaviour
{
    [SerializeField]private ParticleSystem ps_Explode;
    private ParticleSystem ps;
    private BoxCollider boxTrigger;
    public Season season = Season.Spring;
    private void Awake()
    {
        ps = Instantiate(ps_Explode, transform.position, Quaternion.identity);
        ps.transform.SetParent(transform);
        TryGetComponent(out boxTrigger);
    }
    private void OnEnable()
    {
        StartCoroutine(Initiate_Co());
    }
    IEnumerator Initiate_Co()
    {
        if (TryGetComponent(out MeshRenderer renderer))
        {
            renderer.enabled = true;
        }
        float scale = 0.05f;
        float scaleUpDelay = scale;
        WaitForSeconds waitSec = new WaitForSeconds(scaleUpDelay);
        int scaleUpCount = (int)(1f / scaleUpDelay);
        transform.localScale = new Vector3(scale, scale, scale);
        for(int i=0; i<scaleUpCount; i++)
        {
            transform.localScale += new Vector3(scale, scale, scale);
            yield return waitSec;
        }
        yield return new WaitForSeconds(2f);
        Explode();
    }
    public void Explode()
    {
        StartCoroutine(Explode_Co());
    }
    IEnumerator Explode_Co()
    {
        if(TryGetComponent(out MeshRenderer renderer))
        {
            renderer.enabled = false;
        }
        
        ps.Play();
        boxTrigger.enabled = true;
        yield return null;
        boxTrigger.enabled = false;
        yield return new WaitUntil(() => ps.isPlaying);
        yield return new WaitUntil(() => ps.isStopped);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        //
    }
}
