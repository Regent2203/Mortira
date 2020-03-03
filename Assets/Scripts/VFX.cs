using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    public int Number = 10;

    /*
    ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
    */

    void Start()
    {
        GetComponent<ParticleSystem>().Emit(Number); //и так сойдет, не будем усложнять
    }
        
}
