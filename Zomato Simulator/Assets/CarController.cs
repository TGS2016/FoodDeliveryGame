using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    private PlayerInput _input;
    private Rigidbody2D _rb2d;

    public float speed;
    
    public bool canMove { get; set; }


    [Header("Car Turn Properties")]
    [SerializeField] float driftFactor;
    [SerializeField] float turnAmount;

    [SerializeField] Sprite[] car_sprites;

    public SpriteRenderer carSpriteRenderer;


    [SerializeField] private Vector3 drive_offset;
    public Vector3 initial_offset;
    public float current_angle;
    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<PlayerInput>();
        _rb2d = GetComponent<Rigidbody2D>();      
       
        canMove = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();

       
    }


    private void Move()
    {
        if (_input.GetVerticalInput() == 0)
        {
            _rb2d.drag = Mathf.Lerp(_rb2d.drag, 3f, Time.fixedDeltaTime * 3);
            return;
        }
        else
        {
            _rb2d.drag = 0.2f;            
        }

        if (Mathf.Abs(_rb2d.velocity.magnitude) > 0.05)
        {
            Debug.Log("Change Rotation");

            //_rb2d.AddForce((drive_offset).normalized *  _input.GetVerticalInput() * Time.fixedDeltaTime * speed, ForceMode2D.Impulse);
            //_rb2d.SetRotation(_rb2d.rotation +(turnAmount * _input.GetHorizontalInput()));
            ChangeDriveOffset(-(_input.GetHorizontalInput()));
            
        }
        else
        {
            currentTime = 0;
           // _rb2d.AddForce((drive_offset).normalized * _input.GetVerticalInput() * Time.fixedDeltaTime * speed,ForceMode2D.Impulse);

        }

        Vector3 curr_velocity = _rb2d.velocity;
        Vector3 desiredVelocity = drive_offset.normalized;

        _rb2d.velocity = Vector3.Lerp(_rb2d.velocity,desiredVelocity * speed * _input.GetVerticalInput() ,Time.fixedDeltaTime);




        UpdateSpriteAsPerRotation();
    }

   
    [SerializeField] float turn_speed;
    [SerializeField] float currentTime;
    private void ChangeDriveOffset(float inputvalue)
    {
        if (current_angle == 0)
        {
            current_angle = 360;
        }

        currentTime += Time.deltaTime;
        if (currentTime < turn_speed) return;



        if (inputvalue == 0) return;

        currentTime = 0;


        if (inputvalue > 0)
        {
            inputvalue = 1;
        }
        else
        {
            inputvalue = -1;
        }
        Debug.Log(inputvalue);
        current_angle += turnAmount * inputvalue;

        if (current_angle == 0)
        {
            current_angle = 360;
        }


        drive_offset = new Vector3(Mathf.Cos(current_angle * Mathf.Deg2Rad), Mathf.Sin(current_angle * Mathf.Deg2Rad));

        UpdateSpriteAsPerRotation();
    }

    private void UpdateSpriteAsPerRotation()
    {
        int spriteIndex =(int) Mathf.Abs((current_angle%360) / turnAmount);

        carSpriteRenderer.sprite = car_sprites[spriteIndex];
    }

    
}


[System.Serializable]
public class CarInfo
{
    public float carSpeed;
    public float carTurnTime;
    public float maxFuelCapacity;
    public List<CarSprites> allColorSprite = new List<CarSprites>();
}

[System.Serializable]
public class CarSprites
{
    public List<Sprite> car_sprites=new List<Sprite>();
}
