using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathMaker : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<20; i++)
        {
            ParticleSystem obj = Instantiate(ps, transform.position + i*transform.forward, Quaternion.identity);
            Debug.Log(obj.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
