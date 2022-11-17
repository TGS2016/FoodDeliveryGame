using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour,IPunObservable
{
    private PlayerInput _input;
    private Rigidbody2D _rb2d;
    private Animator _animator;
    private PhotonView PV;
    private CarController myCar;
    private Direction _direction;
   

    public float speed;
    public bool canMove;


    //CAR DATA
    public int selected_car;
    public int selected_car_color;

    [SerializeField] Collider2D playerCollider;
    [SerializeField] SpriteRenderer player_sr;

    public PlayerState _pState = PlayerState.WORLD;

    [SerializeField] TextMesh username_txt;
    public bool DisableInputs;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
        

        if (PV.IsMine)
        {
            username_txt.gameObject.SetActive(false);
            GameObject car = PhotonNetwork.Instantiate("Car", this.transform.position + new Vector3(3, 0, 0), Quaternion.identity);

            if (CommonReferences.Instance) {
                
                CommonReferences.Instance.myPlayer = this;
                CommonReferences.Instance.myPV = this.PV;
                CommonReferences.Instance.myCar= myCar = car.GetComponent<CarController>();

                selected_car = DatabaseManager.Instance.GetLocalData().selected_car;
                selected_car_color= DatabaseManager.Instance.GetLocalData().selected_car_color;

                CommonReferences.Instance.myCar.SetupCar(selected_car, selected_car_color);

                CommonReferences.Instance.SetupCameras();
            }

            _animator.SetFloat("GenderID", DatabaseManager.Instance.GetLocalData().char_id);
            UIManager.Instance.SetCoinText();


           
        }
        else
        {
            username_txt.text = PV.Owner.NickName;
        }
        canMove = true;
    }
    async void Start()
    {
        if (PV.IsMine)
        {
            UIManager.Instance.ToggleLoadingPanel(true);

            long lastTimeSpin = long.Parse(DatabaseManager.Instance.GetLocalData().last_spin_time);
            long currentTime = await DatabaseManager.Instance.GetCurrentTime();


            Debug.Log("lastTimeSpin : " + lastTimeSpin);
            Debug.Log("currentTime : " + currentTime);

            long diff = currentTime - lastTimeSpin;

            Debug.Log("difference : " + diff);
            //if (lastTimeSpin != null && currentTime != null)
            {
                //TimeSpan ts = currentTime - lastTimeSpin;
#if UNITY_EDITOR
                diff = 99999;
#endif
                if (diff > 86400)
                {
                   
                    UIManager.Instance.ToggleSpinUI(true);
                }
                else
                {
                    bool tutorialDone = PlayerPrefs.GetInt("TutDone", 0) == 1;
                    if (!tutorialDone)
                    {
                        UIManager.Instance.EnableSteps();
                    }
                }
            }

            UIManager.Instance.ToggleLoadingPanel(false);
        }
    }

    public void ClaimPrize(int prize_index)
    {
        Debug.LogWarning("IMPLEMENT NEW CODE HERE");
        switch (prize_index)
        {
            case 0:
                {
                    //20 Coins
                    LocalData data = DatabaseManager.Instance.GetLocalData();
                    data.coins += 20;
                    UIManager.Instance.UpdatePlayerUIData(true, data);
                    DatabaseManager.Instance.UpdateData(data);
                    UIManager.Instance.SetCoinText();
                    break;
                }
            case 1:
                {
                    //NO LUCK
                    break;
                }
            case 2:
                {
                    LocalData data = DatabaseManager.Instance.GetLocalData();
                    data.coins += 40;
                    UIManager.Instance.UpdatePlayerUIData(true, data);
                    DatabaseManager.Instance.UpdateData(data);
                    UIManager.Instance.SetCoinText();
                    break;
                }
            case 3:
                {

                    break;
                }
            case 4:
                {
                    LocalData data = DatabaseManager.Instance.GetLocalData();
                    data.coins += 100;
                    UIManager.Instance.UpdatePlayerUIData(true, data);
                    DatabaseManager.Instance.UpdateData(data);
                    UIManager.Instance.SetCoinText();
                    break;
                }
            case 5:
                {
                    LocalData data = DatabaseManager.Instance.GetLocalData();
                    data.coins += 200;
                    UIManager.Instance.UpdatePlayerUIData(true, data);
                    DatabaseManager.Instance.UpdateData(data);
                    UIManager.Instance.SetCoinText();

                    break;
                }
        }


       
       
            bool tutorialDone = PlayerPrefs.GetInt("TutDone", 0) == 1;
            if (!tutorialDone)
            {
                UIManager.Instance.EnableSteps();
            }
        
    }
    private void Update()
    {
        if (!PV.IsMine) return;

        if (_input.GetMapKey())
        {
            CommonReferences.Instance.ToggleMap();
        }

        if (DisableInputs) return;
        if (canEnterCar && _input.GetInteractButton())
        {
            canEnterCar = false;
            //Enter Car
            //Debug.Log(_input.GetInteractButton());
            TogglePlayer(false);
            myCar.ToggleCar(true);
            
            CommonReferences.Instance.SwitchCamera(CAMERA_TYPE.CAR);
        }


        Vector2 tempMov = _input.GetPlayerMovement();
        int animatorStateValue = tempMov.magnitude > 0 ? 4 : 0;
       
        if(tempMov.y != 0)
        {
            _direction = tempMov.y > 0 ? Direction.back : Direction.front;
        }
        else if (tempMov.x != 0)
        {
            _direction = tempMov.x > 0 ? Direction.right : Direction.left;
        }

        animatorStateValue += (int)_direction;
        _animator.SetFloat("State", animatorStateValue);

        

#if UNITY_EDITOR
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                selected_car++;
                if (selected_car >= AllCarInfo.Instance.allCarInfo.Count)
                {
                    selected_car = 0;
                }

                myCar.SetupCar(selected_car, selected_car_color);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                selected_car_color++;
                if (selected_car_color >= 8)
                {
                    selected_car_color = 0;
                }
                myCar.SetupCar(selected_car, selected_car_color);
            }
        }
#endif
    }

    
    public bool isPlayerenabled = true;
    public void TogglePlayer(bool enabled)
    {

        //Debug.Log("player toggled" + enabled);

        /*this.gameObject.SetActive(enabled);*/

        if (!enabled)
        {
            if (PV.IsMine)
                this.transform.parent = myCar.transform;
            //CHANGE PLAYER POSITION
        }
        else
        {
            if (PV.IsMine)
                this.transform.parent = null;
        }


        if (enabled)
        {
            if (PV.IsMine)
            {
                Vector3 randomPos = UnityEngine.Random.insideUnitSphere * 2 + Vector3.one;
                randomPos.z = 0;

                Vector3 position = myCar.transform.position + randomPos;
                for (int i = 0; i < 10; i++)
                {
                    if (Physics2D.OverlapBox(position, new Vector2(0.05f, 0.05f), 0) != null)
                    {
                        randomPos = UnityEngine.Random.insideUnitSphere * 2 + Vector3.one;
                        randomPos.z = 0;
                        position = myCar.transform.position + randomPos;
                    }
                    else break;
                }

                this.transform.position = position;
            }
            _pState = PlayerState.WORLD;
        }
        else
        {
            _pState = PlayerState.DRIVE;
        }

        player_sr.enabled = enabled;
        _rb2d.isKinematic = !enabled;
        playerCollider.enabled = enabled;

       

        canMove = enabled;
        isPlayerenabled = enabled;
       
       
    }

    private void FixedUpdate()
    {
        if (PV.IsMine && canMove && !DisableInputs)
        {
            Move();
        }
    }
    
            private void Move()
    {
        _rb2d.MovePosition(_rb2d.position + _input.GetPlayerMovement() * Time.fixedDeltaTime * speed);
    }

    [SerializeField] bool canEnterCar=false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("car") && collision.GetComponent<PhotonView>().IsMine)
        {
            canEnterCar = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("car") && collision.GetComponent<PhotonView>().IsMine)
        {
            canEnterCar = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isPlayerenabled);
        }
        else if (stream.IsReading)
        {
            bool playerEnabled = (bool)stream.ReceiveNext();
            if(playerEnabled != isPlayerenabled)
            {
                Debug.Log("TOGGLEED");
                TogglePlayer(playerEnabled);
            }
        }
    }

    public enum Direction
    {
        back,
        front,
        left,
        right
    }
}
public enum PlayerState
{
    WORLD,DRIVE
}

