using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovementController : MonoBehaviour
{
    public  float   BaseSpeed = 0.05f;
    public AimController Aim;

    event Action OnPlayerMoved;

    //components
    public Transform tr { get; set; }


    void Awake()
    {
        tr = transform;
        if (Aim == null)
        {
            Aim = tr.GetChild(1).GetComponent<AimController>();
            Debug.LogWarning("Player -> MovementContoller -> Aim is not assigned! Fix it in UnityEditor.");
        }
    }

    void Start()
    {        
        OnPlayerMoved += Aim.GetAim;
    }

    void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        if (!Vector3.Equals(_input, Vector3.zero))
            Move();        
    }
    
    Vector3 _input;
    void GetInput()
    {
        _input = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            _input.z = 1;
        else if (Input.GetKey(KeyCode.S))
            _input.z = -1;

        if (Input.GetKey(KeyCode.D))
            _input.x = 1;
        else if (Input.GetKey(KeyCode.A))
            _input.x = -1;

        //_input.y = 0; //always
    }

    void Move()
    {
        tr.position += _input * BaseSpeed;
        OnPlayerMoved?.Invoke();
    }

    public void RotatePlatform(float Angle)
    {                
        tr.localEulerAngles += new Vector3(0, Angle, 0);
    }
}
