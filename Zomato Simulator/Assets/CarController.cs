using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour,IPunObservable
{

    //CAR THEME
    public int currentCar;
    public int currentCarColor;

    public PhotonView pv; 
    private PlayerInput _input;
    private Rigidbody2D _rb2d;

    public float speed;

    public bool canDrive;


    [Header("Car Turn Properties")]
    [SerializeField] float driftFactor;
    [SerializeField] float turnAmount;

    [SerializeField] List<Sprite> car_sprites;

    public SpriteRenderer carSpriteRenderer;


    [SerializeField] private Vector3 drive_offset;
    public Vector3 initial_offset;
    public float current_angle;
    // Start is called before the first frame update
    void Start()
    {
        if (!pv.IsMine) return;
        
            _input = GetComponent<PlayerInput>();
            _rb2d = GetComponent<Rigidbody2D>();

        _rb2d.isKinematic = true;


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!pv.IsMine) return;

        if (canDrive)
        {
            Move();
        }

       
    }

    private void Update()
    {
        if (!pv.IsMine) return;

        if (canDrive && _input.GetInteractButton())
        {

            //EXIT CAR
            ToggleCar(false);            
            CommonReferences.Instance.myPlayer.TogglePlayer(true);
        }
    }

    internal void ToggleCar(bool enabled)
    {
        StartCoroutine(toggleCar(enabled));
        
    }
    IEnumerator toggleCar(bool enabled)
    {
        yield return new WaitForEndOfFrame();
        canDrive = enabled;
        _rb2d.isKinematic = !enabled;
    }


    internal void SetupCar(int selected_car, int selected_car_color)
    {
        currentCar = selected_car;
        currentCarColor = selected_car_color;

        car_sprites = AllCarInfo.Instance.allCarInfo[selected_car].allColorSprite[selected_car_color].car_sprites;
        UpdateSpriteAsPerRotation();
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


    public int currentSpriteIndex=0;
    private void UpdateSpriteAsPerRotation()
    {
        int spriteIndex =(int) Mathf.Abs((current_angle%360) / turnAmount);
        currentSpriteIndex = spriteIndex;
        carSpriteRenderer.sprite = car_sprites[spriteIndex];
    }

    private void UpdateSpriteAsperReceive(int carSpriteIndex)
    {  
        carSpriteRenderer.sprite = car_sprites[carSpriteIndex];
    }


    int local_car_index=0;
    int local_car_color=0;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)currentCar);
            stream.SendNext((int)currentCarColor);
            stream.SendNext((int)currentSpriteIndex);
        }
        else if (stream.IsReading)
        {
            local_car_index = (int) stream.ReceiveNext();
            local_car_color = (int) stream.ReceiveNext();
            currentSpriteIndex = (int) stream.ReceiveNext();

           if(currentCar != local_car_index || currentCarColor != local_car_color)
            {
                currentCar = local_car_index;
                currentCarColor= local_car_color;

                car_sprites = AllCarInfo.Instance.allCarInfo[currentCar].allColorSprite[currentCarColor].car_sprites;


            }

             UpdateSpriteAsperReceive(currentSpriteIndex);

        }
    }
}

