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
    public bool DisableInputs;


    [SerializeField] AudioSource engineSound;
    [SerializeField] float minPitch=0.3f;


    [Header("Car Turn Properties")]
    [SerializeField] float driftFactor;
    [SerializeField] float stoppingTime;
    [SerializeField] float turnAmount;

    [SerializeField] List<Sprite> car_sprites;

    public SpriteRenderer carSpriteRenderer;
    public SpriteRenderer carSpriteRendererTemporary;
    [SerializeField] GameObject colliderObject;
    
    [SerializeField] PolygonCollider2D carCollider;

    [SerializeField] private Vector3 drive_offset;
    public Vector3 initial_offset;
    public float current_angle;



    [Header("Car Fuel")]
    [SerializeField] public float currentFuel;
    [SerializeField] public float maxFuel;



    // Start is called before the first frame update
    void Start()
    {
        if (!pv.IsMine) return;

        _input = GetComponent<PlayerInput>();
        _rb2d = GetComponent<Rigidbody2D>();
        UIManager.Instance.Step[2].ObjectToPoint = this.transform;


        //_rb2d.isKinematic = true;


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!pv.IsMine) return;

        
        if (canDrive)
        {
            Move();
        }
        /*else
        {
            _rb2d.drag = 3f;
        }*/


    }

    private void Update()
    {
        if (!pv.IsMine) return;

        if (canDrive && _input.GetInteractButton() )
        {
            if (_rb2d.velocity.magnitude < 0.1f)
            {
                ToggleCar(false);
                CommonReferences.Instance.myPlayer.TogglePlayer(true);

                canDrive = false;
                CommonReferences.Instance.SwitchCamera(CAMERA_TYPE.PLAYER);
            }
        }

        engineSound.pitch = minPitch + _rb2d.velocity.magnitude / speed;

       
    }

    internal void ToggleCar(bool enabled)
    {
        StartCoroutine(toggleCar(enabled));
        
    }
    IEnumerator toggleCar(bool enabled)
    {
        yield return new WaitForEndOfFrame();
        canDrive = enabled;

        if (canDrive)
        {
            engineSound.Play();
        }
        else
        {
            engineSound.Stop();
        }

        if (pv.IsMine)
        {
            UIManager.Instance.ToggleCarPointerBTN(!enabled);
        }

        UpdateFuelData();
       // _rb2d.isKinematic = !enabled;

        /* if(enabled == false)
         {
             while (_rb2d.velocity != Vector2.zero)
             {
                 //_rb2d.velocity = Vector2.Lerp(_rb2d.velocity, Vector2.zero, stoppingTime);
                 _rb2d.drag = Mathf.Lerp(_rb2d.drag, 3f, Time.fixedDeltaTime * 3);
                 yield return new WaitForEndOfFrame();
             }
         }*/
    }

    public void UpdateFuelData()
    {

        if (pv.IsMine)
        {
            LocalData data = DatabaseManager.Instance.GetLocalData();
            data.all_cars_fuel[currentCar] = currentFuel;
            DatabaseManager.Instance.UpdateData(data);
        }
    }

    int lastSetupCar = -1;

    bool firstTime =true;
    internal void SetupCar(int selected_car, int selected_car_color, bool rechageFuel = true)
    {
        
        
        if (!firstTime)
        {
            UpdateFuelData();
        }

        firstTime = false;
        currentCar = selected_car;
        currentCarColor = selected_car_color;


        if (rechageFuel)
        {
            currentFuel = AllCarInfo.Instance.allCarInfo[selected_car].maxFuelCapacity;
        }

        speed = AllCarInfo.Instance.allCarInfo[selected_car].carSpeed;
        maxFuel = AllCarInfo.Instance.allCarInfo[selected_car].maxFuelCapacity;
        turn_speed=  AllCarInfo.Instance.allCarInfo[selected_car].carTurnTime;
        fuelBurnAmount = AllCarInfo.Instance.allCarInfo[selected_car].FuelBURNAmount;

        currentFuel = DatabaseManager.Instance.GetLocalData().all_cars_fuel[selected_car];


        CommonReferences.Instance.myInventory.BagSize= AllCarInfo.Instance.allCarInfo[selected_car].maxFoodCapacity;

        car_sprites = AllCarInfo.Instance.allCarInfo[selected_car].allColorSprite[selected_car_color].car_sprites;
        UpdateSpriteAsPerRotation();

        lastSetupCar = selected_car;
        if (UIManager.Instance)
        {
            UIManager.Instance.fuelSlider.value = currentFuel / maxFuel;
        }
    }
   
    private void Move()
    {

        if (DisableInputs)
        {
            _rb2d.drag = Mathf.Lerp(_rb2d.drag, 3f, Time.fixedDeltaTime * 3);
            return;
        }

        if (currentFuel <= 0)
        {
            _rb2d.drag = Mathf.Lerp(_rb2d.drag, 3f, Time.fixedDeltaTime * 3);
            return;
        }

        UIManager.Instance.fuelSlider.value = currentFuel / maxFuel;

        if (_input.GetVerticalInput() == 0)
        {
            _rb2d.drag = Mathf.Lerp(_rb2d.drag, 3f, Time.fixedDeltaTime * 3);
            return;
        }
        else
        {
            _rb2d.drag = 0.2f;            
        }

   
        if (Mathf.Abs(_input.GetVerticalInput()) > 0)
        {

            //_rb2d.AddForce((drive_offset).normalized *  _input.GetVerticalInput() * Time.fixedDeltaTime * speed, ForceMode2D.Impulse);
            //_rb2d.SetRotation(_rb2d.rotation +(turnAmount * _input.GetHorizontalInput()));
            ChangeDriveOffset(_input.GetHorizontalInput() , _input.GetVerticalInput());
            
        }
        else
        {
            currentTime = 0;
           // _rb2d.AddForce((drive_offset).normalized * _input.GetVerticalInput() * Time.fixedDeltaTime * speed,ForceMode2D.Impulse);

        }

        Vector3 curr_velocity = _rb2d.velocity;
        Vector3 desiredVelocity = drive_offset.normalized;

       

        if (_input.GetPlayerMovement() != Vector2.zero)
        {
            currentFuel -= Time.deltaTime * fuelBurnAmount;
          

            if (currentFuel < maxFuel / 2)
            {
                StartCoroutine(UIManager.Instance.tutorialCO("low gas"));

            }
            if (currentFuel < maxFuel / 4)
            {
                StartCoroutine(UIManager.Instance.tutorialCO("find gas"));
            }
            if (currentFuel <= 0)
            {
                StartCoroutine(UIManager.Instance.tutorialCO("no gas"));
            }
        }


        _rb2d.velocity = Vector3.Lerp(_rb2d.velocity,desiredVelocity * speed * _input.GetVerticalInput() ,Time.fixedDeltaTime * 5);




        UpdateSpriteAsPerRotation();
    }

   
    [SerializeField] float turn_speed;
    [SerializeField] float fuelBurnAmount;
    [SerializeField] float currentTime;
    private void ChangeDriveOffset(float horizontalValue, float verticalValue)
    {
        if (current_angle == 0)
        {
            current_angle = 360;
        }

        currentTime += Time.deltaTime;
        if (currentTime < turn_speed) return;



        if (horizontalValue == 0) return;

        currentTime = 0;


        if (verticalValue > 0)
        {
            current_angle -= turnAmount * Mathf.RoundToInt(horizontalValue);
        }
        else
        {
            current_angle += turnAmount * Mathf.RoundToInt(horizontalValue);
        }
        

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
        int tempSpriteIndex = currentSpriteIndex;

        currentSpriteIndex = spriteIndex;
        carSpriteRenderer.sprite = car_sprites[spriteIndex];
        carSpriteRendererTemporary.sprite= car_sprites[spriteIndex];
        if (tempSpriteIndex != spriteIndex)
        {
            DestroyImmediate(colliderObject.GetComponent<PolygonCollider2D>());
            colliderObject.AddComponent<PolygonCollider2D>();
        }
    }

    private void UpdateSpriteAsperReceive( int oldSpriteIndex, int newSpriteIndex)
    {
        carSpriteRenderer.sprite = car_sprites[newSpriteIndex];
        carSpriteRendererTemporary.sprite = car_sprites[newSpriteIndex];

        if (oldSpriteIndex != newSpriteIndex)
        {
            DestroyImmediate(colliderObject.GetComponent<PolygonCollider2D>());
            colliderObject.AddComponent<PolygonCollider2D>();
        }
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
            int oldSpriteIndex = currentSpriteIndex;
            currentSpriteIndex = (int) stream.ReceiveNext();


           

           if(currentCar != local_car_index || currentCarColor != local_car_color)
            {
                currentCar = local_car_index;
                currentCarColor= local_car_color;

                car_sprites = AllCarInfo.Instance.allCarInfo[currentCar].allColorSprite[currentCarColor].car_sprites;


            }

             UpdateSpriteAsperReceive(oldSpriteIndex, currentSpriteIndex);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pv.IsMine) return;

        if (collision.CompareTag("gas_station"))
        {
            StartCoroutine(UIManager.Instance.tutorialCO("fill gas"));
            UIManager.Instance.ToggleRechargeButton(true,(int) (maxFuel - currentFuel));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!pv.IsMine) return;

        if (collision.CompareTag("gas_station"))
        {
            UIManager.Instance.ToggleRechargeButton(false);
        }
    }

    public void RechargeFuel()
    {
        float remainingFuel = maxFuel - currentFuel;
        int gasPrice =(int) remainingFuel;
        Debug.Log(gasPrice);

        LocalData data = DatabaseManager.Instance.GetLocalData();
        if (data.coins >= gasPrice)
        {
            data.coins -= gasPrice;
            DatabaseManager.Instance.UpdateData(data);
            UIManager.Instance.SetCoinText();

            currentFuel = maxFuel;
        }
        else
        {
            if (data.coins == 0)
            {
                MessageBox.insta.showMsg("Not Enough Money to buy fuel!", true);
            }
            else
            {
                currentFuel += data.coins;
                data.coins = 0;
                currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);

                DatabaseManager.Instance.UpdateData(data);
                UIManager.Instance.SetCoinText();
                   UpdateFuelData();
            }
        }
        UpdateFuelData();
    }
}

