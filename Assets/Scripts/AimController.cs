using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{    
    public  GameObject  VFX_prefab; //при выстреле
    public  GameObject  BulletPrefab;
    public  Texture2D   AimGreen, AimRed; //курсор
    public  Transform   SpawnPoint;
    public  MovementController Platform;
    [Header("Bullet")]
    public  int     b_Time;
    public  float   b_Grav;

    Vector3 target      { get; set; }
    Vector3 spawnpos    { get { return SpawnPoint.position; } }
    bool    Aimed       { get; set; } = false;

    //components
    LineRenderer lr; //траектория полета пули
    Camera cam;
    Transform tr;


    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        cam = Camera.main;
        tr = transform;

        layer_number = LayerMask.NameToLayer("Terrain");

        if (Platform == null)
        {
            Platform = tr.parent.GetComponent<MovementController>();
            Debug.LogWarning("Player -> GunBase-> Movement is not assigned! Fix it in UnityEditor.");
        }
    }

    void Start()
    {
        GetAim(); //излишне, но пусть будет
    }

    void Update()
    {
        //Проводим артиллерийские расчеты только при сдвиге мыши
        if ((Input.GetAxis("Mouse X") != 0)
            || (Input.GetAxis("Mouse Y") != 0))
                GetAim();

        if (Input.GetMouseButtonDown(1))
            if (Aimed)
            {
                //CalculateBullet();
                Shoot();
            }
    }

    int layer_number;
    public void GetAim()
    {        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);        
        RaycastHit results;
        
        if (Physics.Raycast(ray, out results, layer_number))
        {
            target = results.point;
            CalculateBullet();

            Aimed = true;
            RefreshTrail(true);
            RefreshCursor(true);
            RotateBarell();
        }
        else
        {
            Aimed = false;
            RefreshTrail(false);
            RefreshCursor(false);            
        }
    }

    void RefreshTrail(bool OK)
    {
        if (!OK)
        {
            lr.enabled = false;
            return;
        }

        /*
        //прямая линия
        lr.positionCount = 2;
        lr.SetPosition(0, spawnpos);
        lr.SetPosition(1, target);
        */

        //навесная траектория
        lr.positionCount = b_Time + 1;
        Vector3 temp_pos = spawnpos;
        for (int i = 0; i < lr.positionCount; i++)
        {
            lr.SetPosition(i, temp_pos);
            temp_pos += new Vector3(V.x, V.y - b_Grav * i - b_Grav / 2, V.z);            
        }


        if (!lr.enabled)
            lr.enabled = true;
    }

    void RefreshCursor(bool OK)
    {
        Vector2 hotspot = new Vector2(32, 32);

        if (OK)
            Cursor.SetCursor(AimGreen, hotspot, CursorMode.Auto);
        else
            Cursor.SetCursor(AimRed, hotspot, CursorMode.Auto);
    }

    Vector3 V;
    void CalculateBullet()
    {
        //x=x0+Vx*t;
        //y=y0+Vy*t-gt^2/2;
        //z=z0+Vz*t;        

        V.x = (target.x - spawnpos.x) / (b_Time);        
        V.z = (target.z - spawnpos.z) / (b_Time);        
        V.y = (target.y - spawnpos.y + b_Grav * b_Time * b_Time / 2) / b_Time;
    }

    void RotateBarell()
    {
        //границы наклона ствола орудия
        const float gr_left = -20, gr_right = 20;

        //rotation: x>0 - vpered; x<0 nazad; y<0 vlevo; y>0 vpravo;
        Vector3 t;
        //Vector3 s;

        //влево-вправо
        t = new Vector3(target.x, 0, target.z);
        //s = new Vector3(spawnpos.x, 0, spawnpos.z);
        //float AngleHorz = Vector3.SignedAngle(Vector3.forward, t - s, Vector3.up); //угол в пространстве, относительно направления "на 12 часов" до указателя мыши
        float AnglePlat = Vector3.SignedAngle(Platform.tr.forward, t - Platform.tr.position, Vector3.up); //угол между направлением платформы и указателем мыши
        //Debug.Log(AnglePlat);
        //Debug.Log(t-s);


        float AP = 0; //angle for platform
        float AB = AnglePlat; //angle for barrel (horizontal)
        if (AnglePlat < gr_left)
        {
            AP = AnglePlat - gr_left;
            AB = gr_left;
        }
        else if (AnglePlat > gr_right)
        {
            AP = AnglePlat - gr_right;
            AB = gr_right;
        }

    

        //применяем к платформе
        if (AP != 0)
            Platform.RotatePlatform(AP);
        //применяем к стволу
        tr.localEulerAngles = new Vector3(tr.localEulerAngles.x, AB, 0);
    }

    void Shoot()
    {        
        //для проверки идеальной точности
        /*
        Debug.Log("finish: ");
        Debug.Log(target.x);
        Debug.Log(target.y);
        Debug.Log(target.z);
        */

        var _bullet = Instantiate(BulletPrefab, SpawnPoint.position, Quaternion.identity).GetComponent<Bullet>();
        _bullet.Speed = V;
        _bullet.Gravity = b_Grav;
        _bullet.LifeTime = b_Time;

        ShootEffect();
    }

    void ShootEffect()
    {
        Instantiate(VFX_prefab, spawnpos, Quaternion.identity);
    }
}
