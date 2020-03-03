using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bullet : MonoBehaviour
{
    public  Vector3  Speed       { get; set; }
    public  float    Gravity     { get; set; } //только по Y
    public  int      LifeTime    { get; set; }

    event   Action  OnTargetReached;

    public GameObject VFX_prefab; //визуальный, так сказать, эффект

    //components
    Transform tr;
    //MeshRenderer mr;



    void Awake()
    {
        tr = transform;
        //mr = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        OnTargetReached += ExplodeEffect;
    }

    void FixedUpdate()
    {
        Move();
        ReduceLifetime();
    }

    void Move()
    {
        tr.Translate(Speed + new Vector3(0, -Gravity * 0.5f, 0)); //-gt^2/2
        Speed = new Vector3(Speed.x, Speed.y - Gravity, Speed.z);
    }
    void ReduceLifetime()
    {
        LifeTime--;
        if (LifeTime == -1)
        {
            OnTargetReached?.Invoke();            
            //mr.enabled = false;
        }
    }

    void ExplodeEffect()
    {
        Instantiate(VFX_prefab, tr.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
